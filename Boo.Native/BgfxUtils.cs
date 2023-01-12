using System.Runtime.CompilerServices;

namespace Boo.Native;



internal class BgfxUtils
{
    public static readonly Bgfx.FrameBufferHandle FrameBufferNone = new() { idx = ushort.MaxValue };
    
    public static unsafe Bgfx.Memory* MakeRef<T>(T[] data) where T : struct
    {
        return Bgfx.make_ref(new Memory<T>(data).Pin().Pointer, (uint)((uint)data.Length * Unsafe.SizeOf<T>()));
    }
    
    public static unsafe Bgfx.Memory* MakeRef<T>(Memory<T> data) where T : struct
    {
        return Bgfx.make_ref(data.Pin().Pointer, (uint)((uint)data.Length * Unsafe.SizeOf<T>()));
    }

    public static unsafe Bgfx.Memory* MakeCopy<T>(Memory<T> data) where T : struct
    {
        return Bgfx.copy(data.Pin().Pointer, (uint)((uint)data.Length * Unsafe.SizeOf<T>()));
	}
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bgfx.StateFlags STATE_BLEND_FUNC(Bgfx.StateFlags src,Bgfx.StateFlags dst)
    {
        return STATE_BLEND_FUNC_SEPARATE(src, dst, src, dst);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bgfx.StateFlags STATE_BLEND_FUNC_SEPARATE(Bgfx.StateFlags srcRGB,Bgfx.StateFlags dstRGB,Bgfx.StateFlags srcA,Bgfx.StateFlags dstA)
    {
        return (Bgfx.StateFlags) ((ulong)srcRGB | ((ulong)dstRGB << 4)| (((ulong) srcA | ((ulong) dstA << 4)) << 8));
    }

}