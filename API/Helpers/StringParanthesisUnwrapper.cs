using System;
using System.Collections.Generic;
using System.Text;

namespace API.Helpers
{
  public class StringParanthesisUnwrapper
  {
    /// <summary>
    /// This helper class contains a string <c>name</c> and its child strings (strings contained within one scope of paranthesis and separated by commas)
    /// </summary>
    /// <typeparam name="StringAndChildren"></typeparam>
    private class StringAndChildren : IComparable<StringAndChildren>
    {
      public string name { get; }
      public List<StringAndChildren> children { get; }
      public StringAndChildren(string name, bool hasChildren)
      {
        this.name = name.Trim(); // no leading or trailing whitespace allowed, as the example seems to show
        this.children = hasChildren ? new List<StringAndChildren>() : null;
      }
      public int CompareTo(StringAndChildren other)
      {
        return this.name.CompareTo(other.name);
      }
      public void AddChild(StringAndChildren child)
      {
        if (child.name.Length > 0)
        {
          this.children.Add(child);
        }
        else
        {
          throw new ArgumentException("A child element cannot be an empty string or consist of only white-space");
        }
      }
    }

    /// <summary>
    /// Adds children recursively to the StringAndChildren object passed in, then sorts them
    /// </summary>
    /// <param name="substring"></param>
    /// <param name="stringAndChildren"></param>
    private static void addChildren(string substring, StringAndChildren stringAndChildren)
    {
      StringBuilder childName = new StringBuilder();
      int n = substring.Length;
      if (n == 0)
      {
        throw new ArgumentException("Child elements must be something beyond white space");
      }
      int i = 0;

      // loop through substring
      while (i < n)
      {
        char c = substring[i];

        if (c == ')') // since this is inside a substring, a closing paranthesis is closing one that never opened
        {
          throw new ArgumentException("A closing paranthesis was found too early, and/or there are too many of them");
        }
        else if (c == '(') // if an opening paranthesis is found, find the entire contents / children of this scope of paranthesis
        {
          int start = i + 1;
          int openParans = 0;
          while (i < n)
          {
            c = substring[i];
            if (c == '(')
            {
              openParans++;
            }
            else if (c == ')')
            {
              openParans--;
            }
            if (openParans == 0)
            {
              break;
            }
            i++;
          }
          if (openParans != 0) // more open paranthesis were found than closing ones
          {
            throw new ArgumentException("String has too many opening paranthesis");
          }
          StringAndChildren childStringAndChildren = new StringAndChildren(childName.ToString(), true);
          childName = new StringBuilder();
          addChildren(substring.Substring(start, i - start), childStringAndChildren); // this recursively adds children from the substring contents within this element
          stringAndChildren.AddChild(childStringAndChildren);

          i++; // move past closing paranthesis
          while (i < n && Char.IsWhiteSpace(substring[i]))
          {
            i++; // allow white space to follow element with children
          }
          if (i < n)
          {
            if (substring[i] == ',')
            {
              i++; // skip commas
            }
            else if (substring[i] == ')')
            {
              throw new ArgumentException("String has too many closing paranthesis");
            }
            else
            {
              throw new ArgumentException("Invalid character after element with children");
            }
          }
          continue; // don't auto-increment for the case of an element with children
        }
        else if (c == ',')
        {
          stringAndChildren.AddChild(new StringAndChildren(childName.ToString(), false));
          childName = new StringBuilder();
        }
        else
        {
          childName.Append(c);
        }

        i++;
      }
      if (childName.Length > 0) // if the last element had children, another child won't be added to this parent as it was already added recursively.  Otherwise, need to add in the last element without children before the end of the substring
      {
        stringAndChildren.AddChild(new StringAndChildren(childName.ToString(), false));
      }
      stringAndChildren.children.Sort(); // sort each element that has children
    }

    /// <summary>
    /// Adds the appropriate prefix before the string, based on depth of paranthesis. One dash for every depth beyond inside of the first inner paranthesis
    /// </summary>
    /// <param name="result">The result string builder, which is passed recursively back into this function until it is filled</param>
    /// <param name="depth">How many paranthesis have been opened at this point</param>
    /// <param name="parent">"Parents" might not have children, could be an end child node</param>
    private static void addToResult(StringBuilder result, int depth, StringAndChildren parent)
    {
      string prefix = depth > 1 ? new string('-', depth - 1) + ' ' : "";
      if (parent.name.Length > 0)
      {
        result.Append(prefix);
        result.Append(parent.name);
        result.Append("\r\n");
      }
      if (parent.children != null)
      {
        foreach (var child in parent.children)
        {
          addToResult(result, depth + 1, child);
        }
      }
    }

    /// <summary>
    /// Parses an input string based on paranthesis and commas into a list with dashes to represent the original depth of each element. Commas delineate sibling elements whereas opening and closing paranthesis delineate children of elements, with the root (without a name) element containing them all.
    /// For example, the input string "(id,created,employee(id,firstname,employeeType(id), lastname),location)" should produce the following output string:
    /// <code>
    /// created
    /// employee
    /// - employeeType
    /// -- id
    /// - firstname
    /// - id
    /// - lastname
    /// id
    /// location
    /// </code
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public static string ParseString(string inputString)
    {

      int n = inputString.Length;
      // String must begin and end with paranthesis
      if (n < 2 || inputString[0] != '(' || inputString[n - 1] != ')')
      {
        throw new ArgumentException("String must begin and end with paranthesis");
      }
      StringAndChildren root = new StringAndChildren("", true);
      addChildren(inputString.Substring(1, n - 2), root);
      StringBuilder result = new StringBuilder();
      addToResult(result, 0, root);

      int finalLength = result.Length;
      if (finalLength > 0)
      {
        result.Remove(finalLength - 2, 2); // remove final trailing carriage return and new space characters
      }
      return result.ToString();

    }
  }
}