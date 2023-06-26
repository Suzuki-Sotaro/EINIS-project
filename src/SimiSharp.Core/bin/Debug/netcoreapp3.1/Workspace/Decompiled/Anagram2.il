
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Anagram2.dll
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Anagram2.dll.dll
// MVID: {A8481E86-ABA8-443E-8F5B-08FA33BB51DE}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000002057A570000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Program
       extends [mscorlib]System.Object
{
  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class Program/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<char,int32> '<>9__0_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void Program/'<>c'::.ctor()
      IL_0005:  stsfld     class Program/'<>c' Program/'<>c'::'<>9'
      IL_000a:  ret
    } // end of method '<>c'::.cctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c'::.ctor

    .method assembly hidebysig instance int32 
            '<AnagramStrStr>b__0_0'(char x) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method '<>c'::'<AnagramStrStr>b__0_0'

  } // end of class '<>c'

  .method public hidebysig static bool  AnagramStrStr(string needle,
                                                      string haystack) cil managed
  {
    // Code size       104 (0x68)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<char,int32> Program/'<>c'::'<>9__0_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class Program/'<>c' Program/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 Program/'<>c'::'<AnagramStrStr>b__0_0'(char)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<char,int32>::.ctor(object,
                                                                                        native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<char,int32> Program/'<>c'::'<>9__0_0'
    IL_0020:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<char,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0025:  call       int32 [System.Core]System.Linq.Enumerable::Sum(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>)
    IL_002a:  stloc.0
    IL_002b:  ldc.i4.0
    IL_002c:  stloc.1
    IL_002d:  br.s       IL_0056

    IL_002f:  ldc.i4.0
    IL_0030:  stloc.2
    IL_0031:  ldloc.1
    IL_0032:  stloc.3
    IL_0033:  br.s       IL_0049

    IL_0035:  ldloc.2
    IL_0036:  ldarg.1
    IL_0037:  ldloc.3
    IL_0038:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_003d:  add.ovf
    IL_003e:  stloc.2
    IL_003f:  ldloc.0
    IL_0040:  ldloc.2
    IL_0041:  bne.un.s   IL_0045

    IL_0043:  ldc.i4.1
    IL_0044:  ret

    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  add.ovf
    IL_0048:  stloc.3
    IL_0049:  ldloc.3
    IL_004a:  ldarg.1
    IL_004b:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0050:  blt.s      IL_0035

    IL_0052:  ldloc.1
    IL_0053:  ldc.i4.1
    IL_0054:  add.ovf
    IL_0055:  stloc.1
    IL_0056:  ldloc.1
    IL_0057:  ldarg.1
    IL_0058:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_005d:  ldarg.0
    IL_005e:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0063:  sub.ovf
    IL_0064:  blt.s      IL_002f

    IL_0066:  ldc.i4.0
    IL_0067:  ret
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
