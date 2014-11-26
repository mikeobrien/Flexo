Flexo
=============

[![Nuget](http://img.shields.io/nuget/v/Flexo.svg)](http://www.nuget.org/packages/Flexo/) [![Nuget downloads](http://img.shields.io/nuget/dt/Flexo.svg)](http://www.nuget.org/packages/Flexo/) [![Build Status](https://travis-ci.org/mikeobrien/Flexo.png?branch=master)](https://travis-ci.org/mikeobrien/Flexo) 

<img src="https://raw.github.com/mikeobrien/Flexo/master/misc/logo.png"/> 

Flexo is a simple JSON parser/encoder for .NET and Mono.

Install
------------

Flexo can be found on nuget:

    PM> Install-Package Flexo

Usage
------------

Loading and encoding JSON:

```csharp
var element = JElement.Load("{}");

element.ToString().ShouldEqual("{}");
```

Building out JSON elements:

```csharp
var array = new JElement(RootType.Array);
array.AddArrayValueElement(true).Value.ShouldEqual(true);
array.AddArrayValueElement(5).Value.ShouldEqual(5);
array.AddArrayValueElement("hai").Value.ShouldEqual("hai");
array.AddArrayValueElement(null).Value.ShouldEqual(null);
array.AddArrayElement(ElementType.Object);
array.AddArrayElement(ElementType.Array);

array.Count().ShouldEqual(6);

var @object = new JElement(RootType.Object);
@object.AddMember("boolField", ElementType.Boolean).Name.ShouldEqual("boolField");
@object.AddMember("numberField", ElementType.Number).Name.ShouldEqual("boolField");
@object.AddMember("stringField", ElementType.String).Name.ShouldEqual("stringField");
@object.AddMember("nullField", ElementType.Null).Name.ShouldEqual("nullField");
@object.AddMember("objectField", ElementType.Object).Name.ShouldEqual("objectField");
@object.AddMember("arrayField", ElementType.Array).Name.ShouldEqual("arrayField");

@object.Count().ShouldEqual(6);
```

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! 