using System.Globalization;

namespace SCDataSync.Communication
{
    internal class ScDataSyncClient
    {
        private readonly ScDataSyncCommunicator _communicator;
        private readonly byte[]? _fileByteArray;

        internal ScDataSyncClient(string filePath) : this()
        {
            _fileByteArray = File.ReadAllBytes(filePath);
        }
        internal ScDataSyncClient()
        {
            _communicator = new ScDataSyncCommunicator();
            _fileByteArray = null;
        }

        internal void Run()
        {
            _communicator.PrintInformation();
            _communicator.Connect();
            _communicator.UpdatePingStart();

            var sendDataArray = _fileByteArray ?? GetUserInput();
            var dumpDataBuffer = new byte[sendDataArray.Length];

            if (sendDataArray.Length == 0)
            {
                throw new Exception("data size is 0");
            }

            _communicator.SendData(sendDataArray, 0);
            _communicator.WaitForPendingResponse();
            _communicator.CheckDataValidAndResend(sendDataArray, dumpDataBuffer);
            _communicator.WaitForCompleteResponse();
            _communicator.UpdatePingStop();
            CreateDumpFile(dumpDataBuffer);

            Console.WriteLine("done");
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static byte[] GetUserInput()
        {
            Console.WriteLine();
            Console.WriteLine("Select the manual input type:");
            Console.WriteLine("1. 4byte array (uint)");
            Console.WriteLine("2. byte array (hex)");

            byte[] userInputData = Array.Empty<byte>();
            bool validInput = false;

            while (!validInput)
            {
                var key = Console.ReadKey();

                switch (key.KeyChar)
                {
                    case '1':
                        userInputData = GetUIntArrayInput();
                        validInput = true;
                        break;
                    case '2':
                        userInputData = GetHexByteArrayInput();
                        validInput = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("please select 1 or 2");
                        break;
                }
            }

            return userInputData;
        }

        private static byte[] GetUIntArrayInput()
        {
            var uintArray = Array.Empty<uint>();
            var validInput = false;

            while (!validInput)
            {
                Console.WriteLine();
                Console.Write("enter 4byte array (uint):");
                var input = Console.ReadLine() ?? "";
                var inputArray = input.Split(' ');
                if (inputArray.Length == 0)
                    continue;

                uintArray = new uint[inputArray.Length];
                validInput = true;

                for (int i = 0; i < inputArray.Length; i++)
                {
                    if (!uint.TryParse(inputArray[i], out uintArray[i]))
                    {
                        Console.WriteLine("invalid input");
                        validInput = false;
                        break;
                    }
                }
            }
            var byteArray = new byte[uintArray.Length * sizeof(uint)];
            Buffer.BlockCopy(uintArray, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }

        private static byte[] GetHexByteArrayInput()
        {
            var byteArray = Array.Empty<byte>();
            var validInput = false;

            while (!validInput)
            {
                Console.WriteLine();
                Console.Write("enter byte array (hex):");
                var input = Console.ReadLine()?.Replace(" ", "") ?? "";
                if (input.Length % 2 != 0 || input.Length < 2)
                {
                    Console.WriteLine("invalid input length");
                    continue;
                }

                var byteCount = input.Length / 2;
                byteArray = new byte[byteCount];
                validInput = true;

                for (int i = 0; i < byteCount; i++)
                {
                    var hexSubstring = input.Substring(i * 2, 2);
                    if (!byte.TryParse(hexSubstring, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byteArray[i]))
                    {
                        Console.WriteLine("invalid input");
                        validInput = false;
                        break;
                    }
                }
            }
            return byteArray;
        }

        private static void CreateDumpFile(byte[] dumpDataBuffer)
        {
            Console.WriteLine("creating a dump file");
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(path, "dump.dat");
            using var fileStream = new FileStream(filePath, FileMode.Create);
            using var memoryStream = new MemoryStream(dumpDataBuffer);
            memoryStream.CopyTo(fileStream);
        }
    }
}
