// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

using Internal.TypeSystem;

using Debug = System.Diagnostics.Debug;

namespace Internal.IL.Stubs
{
    /// <summary>
    /// Provides method bodies for generic Interlocked intrinsics. These intrinsics work around the lack of byref locals 
    /// return values in C#. The intrinsic method forwards the call to the non-generic version.
    /// </summary>
    public static class InterlockedIntrinsic
    {
        public static MethodIL EmitIL(MethodDesc target)
        {
            Debug.Assert(target.Name == "CompareExchange" || target.Name == "Exchange");

            //
            // Find non-generic method to forward the generic method to.
            //

            int parameterCount = target.Signature.Length;
            Debug.Assert(parameterCount == 3 || parameterCount == 2);

            var objectType = target.Context.GetWellKnownType(WellKnownType.Object);

            var parameters = new TypeDesc[parameterCount];
            parameters[0] = objectType.MakeByRefType();
            for (int i = 1; i < parameters.Length; i++)
                parameters[i] = objectType;

            MethodSignature nonGenericSignature = new MethodSignature(MethodSignatureFlags.Static, 0, objectType, parameters);

            MethodDesc nonGenericMethod = target.OwningType.GetMethod(target.Name, nonGenericSignature);

            // TODO: Better exception type. Should be: "CoreLib doesn't have a required thing in it".
            if (nonGenericMethod == null)
                throw new NotImplementedException();

            //
            // Emit the forwarder
            //

            ILEmitter emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            // Reload all arguments
            for (int i = 0; i < parameterCount; i++)
                codeStream.EmitLdArg(i);

            codeStream.Emit(ILOpcode.call, emitter.NewToken(nonGenericMethod));
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }
    }
}
