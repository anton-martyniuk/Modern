﻿using System;

namespace Modern.Controllers.SourceGenerators;

/// <summary>
/// An attribute that specifies that the property should be excluded
/// from the CreateRequest class generated by the source generator
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreCreateRequestAttribute : Attribute
{
}