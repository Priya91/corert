// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using global::System;
using global::System.Reflection;
using global::System.Diagnostics;
using global::System.Collections.Generic;
using global::System.Reflection.Runtime.TypeInfos;
using global::System.Reflection.Runtime.ParameterInfos;

using global::Internal.Reflection.Core.Execution;
using global::Internal.Reflection.Core.NonPortable;

using global::Internal.Metadata.NativeFormat;

namespace System.Reflection.Runtime.MethodInfos
{
    //
    // The runtime's implementation of constructors exposed on array types.
    //
    internal sealed partial class RuntimeSyntheticConstructorInfo : RuntimeConstructorInfo
    {
        private RuntimeSyntheticConstructorInfo(SyntheticMethodId syntheticMethodId, RuntimeType declaringType, RuntimeType[] runtimeParameterTypesAndReturn, InvokerOptions options, Func<Object, Object[], Object> invoker)
        {
            _syntheticMethodId = syntheticMethodId;
            _declaringType = declaringType;
            _options = options;
            _invoker = invoker;
            _runtimeParameterTypesAndReturn = runtimeParameterTypesAndReturn;
        }

        public sealed override MethodAttributes Attributes
        {
            get
            {
                return MethodAttributes.Public | MethodAttributes.PrivateScope | MethodAttributes.RTSpecialName;
            }
        }

        public sealed override CallingConventions CallingConvention
        {
            get
            {
                return CallingConventions.Standard | CallingConventions.HasThis;
            }
        }

        public sealed override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return Empty<CustomAttributeData>.Enumerable;
            }
        }

        public sealed override Type DeclaringType
        {
            get
            {
                return _declaringType;
            }
        }

        public sealed override Object Invoke(Object[] parameters)
        {
            if (parameters == null)
                parameters = Array.Empty<Object>();

            Object ctorAllocatedObject = this.MethodInvoker.Invoke(null, parameters);
            return ctorAllocatedObject;
        }

        public sealed override MethodImplAttributes MethodImplementationFlags
        {
            get
            {
                return MethodImplAttributes.IL;
            }
        }

        public sealed override String Name
        {
            get
            {
                return ConstructorName;
            }
        }

        public sealed override bool Equals(object obj)
        {
            RuntimeSyntheticConstructorInfo other = obj as RuntimeSyntheticConstructorInfo;
            if (other == null)
                return false;
            if (this._syntheticMethodId != other._syntheticMethodId)
                return false;
            if (!(this._declaringType.Equals(other._declaringType)))
                return false;
            return true;
        }

        public sealed override int GetHashCode()
        {
            return this._declaringType.GetHashCode();
        }

        public sealed override String ToString()
        {
            return RuntimeMethodCommon.ComputeToString(this, Array.Empty<RuntimeType>(), RuntimeParametersAndReturn);
        }

        protected sealed override RuntimeParameterInfo[] RuntimeParametersAndReturn
        {
            get
            {
                RuntimeParameterInfo[] runtimeParametersAndReturn = _lazyRuntimeParametersAndReturn;
                if (runtimeParametersAndReturn == null)
                {
                    runtimeParametersAndReturn = new RuntimeParameterInfo[_runtimeParameterTypesAndReturn.Length];
                    for (int i = 0; i < runtimeParametersAndReturn.Length; i++)
                    {
                        runtimeParametersAndReturn[i] = RuntimeSyntheticParameterInfo.GetRuntimeSyntheticParameterInfo(this, i - 1, _runtimeParameterTypesAndReturn[i]);
                    }
                    _lazyRuntimeParametersAndReturn = runtimeParametersAndReturn;
                }
                return runtimeParametersAndReturn;
            }
        }

        protected sealed override MethodInvoker UncachedMethodInvoker
        {
            get
            {
                RuntimeTypeHandle[] runtimeParameterTypeHandles = new RuntimeTypeHandle[_runtimeParameterTypesAndReturn.Length - 1];
                for (int i = 1; i < _runtimeParameterTypesAndReturn.Length; i++)
                    runtimeParameterTypeHandles[i - 1] = _runtimeParameterTypesAndReturn[i].TypeHandle;
                return ReflectionCoreExecution.ExecutionEnvironment.GetSyntheticMethodInvoker(
                    _declaringType.TypeHandle,
                    runtimeParameterTypeHandles,
                    _options,
                    _invoker);
            }
        }

        private volatile RuntimeParameterInfo[] _lazyRuntimeParametersAndReturn;

        private SyntheticMethodId _syntheticMethodId;
        private RuntimeType _declaringType;
        private RuntimeType[] _runtimeParameterTypesAndReturn;
        private InvokerOptions _options;
        private Func<Object, Object[], Object> _invoker;
    }
}
