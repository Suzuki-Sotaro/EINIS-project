
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Anagram1.dll
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Anagram1.dll.dll
// MVID: {B89BEBAB-79CA-44CE-B1EA-3C56E871C344}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002BE2F2C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Program
       extends [mscorlib]System.Object
{
  .method public hidebysig static bool  AnagramStrStr(string needle,
                                                      string haystack) cil managed
  {
    // Code size       73 (0x49)
    .maxstack  3
    .locals init (bool V_0,
             string V_1,
             int32 V_2,
             char V_3,
             int32 V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldc.i4.0
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_003e

    IL_0008:  ldloc.1
    IL_0009:  ldloc.2
    IL_000a:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_000f:  stloc.3
    IL_0010:  ldarg.0
    IL_0011:  ldloc.3
    IL_0012:  callvirt   instance int32 [mscorlib]System.String::IndexOf(char)
    IL_0017:  stloc.s    V_4
    IL_0019:  ldloc.s    V_4
    IL_001b:  ldc.i4.m1
    IL_001c:  ble.s      IL_0035

    IL_001e:  ldc.i4.1
    IL_001f:  stloc.0
    IL_0020:  ldarg.0
    IL_0021:  ldloc.s    V_4
    IL_0023:  ldc.i4.1
    IL_0024:  callvirt   instance string [mscorlib]System.String::Remove(int32,
                                                                         int32)
    IL_0029:  starg.s    needle
    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0031:  brtrue.s   IL_003a

    IL_0033:  ldc.i4.1
    IL_0034:  ret

    IL_0035:  ldloc.0
    IL_0036:  brfalse.s  IL_003a

    IL_0038:  ldc.i4.0
    IL_0039:  ret

    IL_003a:  ldloc.2
    IL_003b:  ldc.i4.1
    IL_003c:  add
    IL_003d:  stloc.2
    IL_003e:  ldloc.2
    IL_003f:  ldloc.1
    IL_0040:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0045:  blt.s      IL_0008

    IL_0047:  ldc.i4.0
    IL_0048:  ret
  } // end of method Program::AnagramStrStr

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Program::.ctor

} // end of class Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
