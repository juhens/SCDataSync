using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCDataSync.Memory
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _1
    {
        public byte v0;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _2
    {
        public _1 v0;
        public _1 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _4
    {
        public _2 v0;
        public _2 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _8
    {
        public _4 v0;
        public _4 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _16
    {
        public _8 v0;
        public _8 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _24
    {
        public _16 v0;
        public _8 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _32
    {
        public _16 v0;
        public _16 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _64
    {
        public _32 v0;
        public _32 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _128
    {
        public _64 v0;
        public _64 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _256
    {
        public _128 v0;
        public _128 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _512
    {
        public _256 v0;
        public _256 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _1024
    {
        public _512 v0;
        public _512 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _2048
    {
        public _1024 v0;
        public _1024 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _4096
    {
        public _2048 v0;
        public _2048 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _8192
    {
        public _4096 v0;
        public _4096 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _16384
    {
        public _8192 v0;
        public _8192 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _32768
    {
        public _16384 v0;
        public _16384 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _65536
    {
        public _32768 v0;
        public _32768 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _131072
    {
        public _65536 v0;
        public _65536 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _262144
    {
        public _131072 v0;
        public _131072 v1;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _524288
    {
        public _262144 v0;
        public _262144 v1;
    }
}
