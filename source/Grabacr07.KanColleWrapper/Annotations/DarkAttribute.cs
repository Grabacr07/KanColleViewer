/*
 * Elemental Annotations <https://github.com/takeshik/ElementalAnnotations>
 * Copyright © 2015 Takeshi KIRIYA (aka takeshik) <takeshik@tksk.io>
 * Licensed under the zlib License; for details, see the website.
 */

using System;
using System.Diagnostics;

// ReSharper disable CheckNamespace
#if ELEMENTAL_ANNOTATIONS_DEFAULT_NAMESPACE
namespace Elemental.Annotations
#else
namespace Grabacr07.KanColleWrapper.Annotations
#endif
// ReSharper restore CheckNamespace
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    [Conditional("DEBUG")]
    public class DarkAttribute
        : ElementalAttribute
    {
        public const string Name = "Dark";

        public DarkAttribute(string description = null)
            : base(description)
        {
        }
    }

    partial class CodeElement
    {
        public const string Dark = DarkAttribute.Name;
    }
}
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
