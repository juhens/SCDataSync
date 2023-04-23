using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory.Extensions;
using SCDataSync.Memory.Native;

namespace SCDataSync.Memory
{
    internal class JhMemory
    {
        private readonly Process _process;
        internal readonly Dictionary<string, ModuleInformation> ModulesDic;

        private ulong _minAddress, _maxAddress;

        internal JhMemory(Process process)
        {
            _process = process;
            if (process == null)
                throw new Exception("can not found process");
            if (process.MainModule == null)
                throw new Exception("failed get main module information");

            var comparer = StringComparer.OrdinalIgnoreCase;
            ModulesDic = new Dictionary<string, ModuleInformation>(comparer);
            if (!MakeModuleDic())
                throw new Exception("failed make module list");

            SetScanRange();
        }
        //Init
        private bool MakeModuleDic()
        {
            var modulesPtr = Array.Empty<nint>();

            if (!Psapi.EnumProcessModulesEx(_process.Handle, modulesPtr, 0, out var bytesNeeded, (uint)Enums.ModuleFilter.ListModulesAll))
                return false;

            var modulesCount = bytesNeeded / nint.Size;

            modulesPtr = new nint[modulesCount];

            if (Psapi.EnumProcessModulesEx(_process.Handle, modulesPtr, bytesNeeded, out _, (uint)Enums.ModuleFilter.ListModulesAll))
            {
                for (var i = 0; i < modulesCount; i++)
                {
                    var moduleFilePath = new StringBuilder(1024);
                    Psapi.GetModuleFileNameEx(_process.Handle, modulesPtr[i], moduleFilePath, (uint)moduleFilePath.Capacity);

                    Psapi.GetModuleInformation(_process.Handle, modulesPtr[i], out var mi, (uint)(nint.Size * modulesPtr.Length));
                    string moduleName = Path.GetFileName(moduleFilePath.ToString());

                    if (ModulesDic.ContainsKey(moduleName))
                    {
                        ModulesDic.Add("_" + moduleName, mi);
                    }
                    else
                    {
                        ModulesDic.Add(moduleName, mi);
                    }
                }
            }
            return true;
        }


        //All cache
        private IEnumerable<MemoryPage> GetMemoryPages()
        {
            MemoryBasicInformation mbi = new();
            ulong minAddress = _minAddress;
            ulong maxAddress = _maxAddress;

            while (minAddress < maxAddress)
            {
                Kernel32.VirtualQueryEx(_process.Handle, minAddress, out mbi, (uint)Marshal.SizeOf(mbi));
                if (mbi is { Protect: (uint)Enums.MEM_PROTECTION.PAGE_READWRITE, State: (uint)Enums.MEM_ALLOCATION_TYPE.MEM_COMMIT })
                {
                    var raw = new byte[mbi.RegionSize];
                    Read(mbi.BaseAddress, raw.AsByteSpan());
                    yield return new MemoryPage(mbi.BaseAddress, raw);
                }
                if (mbi.RegionSize == 0)
                    throw new Exception("maybe process terminated");
                minAddress += mbi.RegionSize;
            }
        }

        //Setup
        internal void SetScanRange()
        {
            Kernel32.GetSystemInfo(out var si);
            _minAddress = (ulong)si.minimumApplicationAddress;
            _maxAddress = (ulong)si.maximumApplicationAddress;
        }


        //Memory RW
        internal bool Read(ulong address, Span<byte> byteSpan)
        {
            return Kernel32.ReadProcessMemory(_process.Handle, address, ref MemoryMarshal.GetReference(byteSpan), (nuint)byteSpan.Length, out _);
        }
        internal bool Write(ulong address, Span<byte> byteSpan)
        {
            return Kernel32.WriteProcessMemory(_process.Handle, address, ref MemoryMarshal.GetReference(byteSpan), (nuint)byteSpan.Length, out _);
        }

        //Scan
        internal ulong? Scan(byte[] valueArr)
        {
            var patternData = new PatternData(valueArr);
            var result = Compare(patternData);
            GC.Collect();
            return result;
        }

        private ulong? Compare(PatternData pattern)
        {
            var memoryPages = GetMemoryPages();

            ulong? result = null;
            foreach (var memoryPage in memoryPages)
            {
                for (var i = 0; i <= memoryPage.ByteArray.Length - pattern.ByteArray.Length;)
                {
                    for (var j = pattern.ByteArray.Length - 1; j >= 0; j--)
                        if (pattern.ByteArray[j] != memoryPage.ByteArray[i + j])
                            goto Pass;

                    result = memoryPage.BaseAddress + (uint)i;
                    break;
                Pass:
                    i += pattern.JumpTable[memoryPage.ByteArray[i + pattern.ByteArray.Length - 1]];
                }
            }
            return result;
        }
    }
}
