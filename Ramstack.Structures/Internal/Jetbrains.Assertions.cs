// MIT License
//
// Copyright (c) 2016 JetBrains http://www.jetbrains.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#nullable disable

#pragma warning disable 1591

// ReSharper disable CheckNamespace
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace JetBrains.Annotations;

/// <summary>
/// Tells the code analysis engine if the parameter is completely handled when the invoked method is on stack.
/// If the parameter is a delegate, indicates that the delegate can only be invoked during method execution
/// (the delegate can be invoked zero or multiple times, but not stored to some field and invoked later,
/// when the containing method is no longer on the execution stack).
/// If the parameter is an enumerable, indicates that it is enumerated while the method is executed.
/// If <see cref="RequireAwait"/> is true, the attribute will only take effect if the method invocation is located under the 'await' expression.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional("JETBRAINS_ANNOTATIONS")]
internal sealed class InstantHandleAttribute : Attribute
{
    /// <summary>
    /// Require the method invocation to be used under the 'await' expression for this attribute to take effect on the code analysis engine.
    /// Can be used for delegate/enumerable parameters of 'async' methods.
    /// </summary>
    public bool RequireAwait { get; set; }
}

/// <summary>
/// Indicates that the method is a pure LINQ method, with postponed enumeration (like Enumerable.Select,
/// .Where). This annotation allows inference of [InstantHandle] annotation for parameters
/// of delegate type by analyzing LINQ method chains.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[Conditional("JETBRAINS_ANNOTATIONS")]
internal sealed class LinqTunnelAttribute : Attribute { }
