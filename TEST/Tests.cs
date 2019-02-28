using System;
using Xunit;
using API.Helpers;

public class Tests
{
  [Fact]
  public void StringMustHaveOpeningAndClosingParanthesis()
  {
    string[] missingParans = { "bob,mary,joe)", "(bob,mary,joe", "bob,mary,joe" };
    foreach (string missingParan in missingParans)
    {
      var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(missingParan));
      Assert.IsType<ArgumentException>(exception);
    }
  }

  [Fact]
  public void StringCannotHaveExtraOpeningOrClosingParanthesis()
  {
    string[] extraParans = { "((bob,mary,joe)", "(bob,mary,joe))" };
    foreach (string extraParan in extraParans)
    {
      var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(extraParan));
      Assert.IsType<ArgumentException>(exception);
    }
  }

  [Fact]
  public void StringCannotHaveEmptyElements()
  {
    string[] emptyElements = { "(bob,mary,joe,,)", "(bob,mary,joe, ,)", "(bob,mary,joe, (empty,children))", "(bob,mary,joe,(empty,children))" };
    foreach (string emptyElement in emptyElements)
    {
      var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(emptyElement));
      Assert.IsType<ArgumentException>(exception);
    }
  }

  [Fact]
  public void ElementsCanHaveLeadingAndTrailingWhitespaceButItIsRemoved()
  {
    string input = "(  a,  b  (c  ,  d  )  ,  e )";
    string expected = @"a
b
- c
- d
e";
    Assert.Equal(expected, StringParanthesisUnwrapper.ParseString(input));
  }

  [Fact]
  public void ElementsIncludingRootMustHaveChildren()
  {
    string[] emptyChildrenStrings = { "()", "(childlessBob())" };
    foreach (string emptyChildren in emptyChildrenStrings)
    {
      var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(emptyChildren));
      Assert.IsType<ArgumentException>(exception);
    }
  }


  [Fact]
  public void MustHaveCommasBetweenAnElementWithChildrenAndItsNextSibling()
  {
    string noCommaSibling = "(bob,mary(john)dave(linda))";
    var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(noCommaSibling));
    Assert.IsType<ArgumentException>(exception);
  }

  [Fact]
  public void ElementWithChildrenCanHaveSpacesAfter()
  {
    string hasSpacesAfterElement = "(bob,mary(john)  ,dave(linda))";
    var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(hasSpacesAfterElement));
    Assert.Null(exception);
  }


  [Fact]
  public void ElementWithChildrenCannotHaveExtraNonCommaOrClosingParanthesisOrWhitespaceCharactersAfterChildren()
  {
    string[] extraCharsAfterChildren = { "(bob,mary(john)a)", "(bob,mary(john)foo,dave(linda))" };
    foreach (string extraChars in extraCharsAfterChildren)
    {
      var exception = Record.Exception(() => StringParanthesisUnwrapper.ParseString(extraChars));
      Assert.IsType<ArgumentException>(exception);
    }
  }

  [Fact]
  public void OriginalTestCaseOutputsExpectedResult()
  {
    string input = "(id,created,employee(id,firstname,employeeType(id), lastname),location)";
    string expected = @"created
employee
- employeeType
-- id
- firstname
- id
- lastname
id
location";
    Assert.Equal(expected, StringParanthesisUnwrapper.ParseString(input));
  }

  [Fact]
  public void SortsInputAtEachLevelOfDepth()
  {
    string input = "(d,c,a,b(c,b,a(b,c,a)))";
    string expected = @"a
b
- a
-- a
-- b
-- c
- b
- c
c
d";
    Assert.Equal(expected, StringParanthesisUnwrapper.ParseString(input));
  }

}

