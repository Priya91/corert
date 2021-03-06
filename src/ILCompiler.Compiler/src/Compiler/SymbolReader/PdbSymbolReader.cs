// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace ILCompiler.SymbolReader
{
    /// <summary>
    ///  IL sequence point record
    /// </summary>
    public struct ILSequencePoint
    {
        public int Offset;
        public string Document;
        public int LineNumber;
        // TODO: The remaining info
    }

    /// <summary>
    ///  IL local variable debug record
    /// </summary>
    public struct ILLocalVariable
    {
        public int Slot;
        public string Name;
        public bool CompilerGenerated;
    }

    /// <summary>
    /// Abstraction for reading Pdb files
    /// </summary>
    public abstract class PdbSymbolReader : IDisposable
    {
        public abstract IEnumerable<ILSequencePoint> GetSequencePointsForMethod(int methodToken);
        public abstract IEnumerable<ILLocalVariable> GetLocalVariableNamesForMethod(int methodToken);
        public abstract void Dispose();
    }
}
