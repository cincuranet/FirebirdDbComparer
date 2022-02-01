using System;
using System.Collections.Generic;

using FirebirdDbComparer.Common;
using FirebirdDbComparer.Common.Equatable;

using NUnit.Framework;

namespace FirebirdDbComparer.Tests.Common.Equatable;

[TestFixture]
public class EquatableHelperTests
{
    class EquatableEqualsWasCalled : IEquatable<EquatableEqualsWasCalled>
    {
        public bool Result { get; set; }

        public bool Equals(EquatableEqualsWasCalled other)
        {
            Result = true;
            return default;
        }
    }

    class PropertiesEqualClass
    {
        public int Int { get; set; }
        public bool Bool { get; set; }
        public string String { get; set; }
        public List<double> ListOfDouble { get; set; }
        public List<string> ListOfString { get; set; }
    }

    static IEnumerable<TestCaseData> ElementaryEqualsSource()
    {
        yield return new TestCaseData(null, null).Returns(true);
        yield return new TestCaseData(null, new object()).Returns(false);
        yield return new TestCaseData(new object(), null).Returns(false);
        yield return new TestCaseData(new object(), new object()).Returns(null);
        var obj = new object();
        yield return new TestCaseData(obj, obj).Returns(true);
    }
    [TestCaseSource(nameof(ElementaryEqualsSource))]
    public bool? ElementaryEquals(object x, object y)
    {
        return EquatableHelper.ElementaryEquals<object>(x, y);
    }

    [Test]
    public void ElementaryEqualsThenEquatableEquals_CallsEquatableEquals()
    {
        var @this = new EquatableEqualsWasCalled();
        var obj = new EquatableEqualsWasCalled();
        EquatableHelper.ElementaryEqualsThenEquatableEquals(@this, obj);
        Assert.That(@this.Result, Is.True);
    }

    [Test]
    public void PropertiesEqual_SimpleScalars()
    {
        var obj = new PropertiesEqualClass() { Int = 10, Bool = true };
        var other = new PropertiesEqualClass() { Int = 10, Bool = true };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.Int, nameof(PropertiesEqualClass.Int)), new EquatableProperty<PropertiesEqualClass>(x => x.Bool, nameof(PropertiesEqualClass.Bool)));
        Assert.That(result, Is.True);
    }

    [Test]
    public void PropertiesEqual_OnlyDeclaredProperties()
    {
        var obj = new PropertiesEqualClass() { Int = 10, Bool = true };
        var other = new PropertiesEqualClass() { Int = 11, Bool = true };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.Bool, nameof(PropertiesEqualClass.Bool)));
        Assert.That(result, Is.True);
    }

    [Test]
    public void PropertiesEqual_String()
    {
        var obj = new PropertiesEqualClass() { String = "FooBar" };
        var other = new PropertiesEqualClass() { String = "FooBar" };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.String, nameof(PropertiesEqualClass.String)));
        Assert.That(result, Is.True);
    }

    [Test]
    public void PropertiesEqual_StringCS()
    {
        var obj = new PropertiesEqualClass() { String = "FooBar" };
        var other = new PropertiesEqualClass() { String = "foobar" };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.String, nameof(PropertiesEqualClass.String)));
        Assert.That(result, Is.False);
    }

    [Test]
    public void PropertiesEqual_StringOrdinal()
    {
        var obj = new PropertiesEqualClass() { String = "Straße" };
        var other = new PropertiesEqualClass() { String = "Strasse" };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.String, nameof(PropertiesEqualClass.String)));
        Assert.That(result, Is.False);
    }

    [Test]
    public void PropertiesEqual_MatchingSets()
    {
        var obj = new PropertiesEqualClass() { ListOfDouble = new List<double>() { 1.1, 2.3, 4.3, 7.6 } };
        var other = new PropertiesEqualClass() { ListOfDouble = new List<double>() { 2.3, 7.6, 1.1, 4.3 } };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.ListOfDouble, nameof(PropertiesEqualClass.ListOfDouble)));
        Assert.That(result, Is.True);
    }

    [Test]
    public void PropertiesEqual_SubsetSets()
    {
        var obj = new PropertiesEqualClass() { ListOfDouble = new List<double>() { 1.1, 2.3 } };
        var other = new PropertiesEqualClass() { ListOfDouble = new List<double>() { 1.1 } };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.ListOfDouble, nameof(PropertiesEqualClass.ListOfDouble)));
        Assert.That(result, Is.False);
    }

    [Test]
    public void PropertiesEqual_SupersetSets()
    {
        var obj = new PropertiesEqualClass() { ListOfDouble = new List<double>() { 1.1 } };
        var other = new PropertiesEqualClass() { ListOfDouble = new List<double>() { 1.1, 2.3 } };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.ListOfDouble, nameof(PropertiesEqualClass.ListOfDouble)));
        Assert.That(result, Is.False);
    }

    [Test]
    public void PropertiesEqual_StringOrdinalSets()
    {
        var obj = new PropertiesEqualClass() { ListOfString = new List<string>() { "Straße" } };
        var other = new PropertiesEqualClass() { ListOfString = new List<string>() { "Strasse" } };
        var result = EquatableHelper.PropertiesEqual(obj, other, new EquatableProperty<PropertiesEqualClass>(x => x.ListOfString, nameof(PropertiesEqualClass.ListOfString)));
        Assert.That(result, Is.False);
    }
}
