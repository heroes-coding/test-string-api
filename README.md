# Test Parser API

I created this for a simple test for a job, in order to start to:

- Show I can work in C# even though I have never created any kind of .NET application
- Demonstrate the basic ability to set up a simple API
- Fulfill the other requirements of the test
- Gain a little Docker knowledge, as it is probably a part of your CI-CD

I chose to build a .NET Core application since it seems like the most flexible and appropriate for CI-CD and deployment to clients. I use XUnit for testing because it was taking too much time getting NUnit to work with net core.

The result is a very simple API, that exposes the following endpoint: `api/string/{input-string}`.

Where {input-string} is of the following format (including the paranthesis):

```
(id,created,employee(id,firstname,employeeType(id), lastname),location)
```

The names of the elements are unimportant, what's important to know is that a comma delineates the next sibling element, and a set of paranthesis (outermost) delineates the children of a preceding element by name.

Here are my assumptions:

- The string must start and end with paranthesis
- There must be an equal number of opening and closing paranthesis (and the corresponding opening ones must come first)
- Empty elements (elements without whitespace) are not okay
- Elements without children (for example, `(childlessBob())`) are not okay
- Elements with leading or trailing white space can be reduced to an element without leading or trailing white space (for example, " lastname" above would become "lastname")

I created an API solution as well as a testing solution, that both are built and run by the Dockerfile.

The docker build command (run from the command line with "`docker build -t dotnetapp-dev .`") does the following:

- Builds the test and api projects
- Runs the tests, displaying the passing output

Running the container (with "`docker run -p {PORT_TO_EXPOSE}:80 --rm dotnetapp-dev`") allows additional usage / testing of the api via standard HTTP get requests to:

```http
http://localhost:{PORT_TO_EXPOSE}/api/string/{input-string}
```

If the string is an appropriate input, it will return a list with dashes preceeding elements that are inside of paranthesis beyond the outermost set of paranthesis.

For example, the input string `(id,created,employee(id,firstname,employeeType(id), lastname),location)` becomes:

```
created
employee
- employeeType
-- id
- firstname
- id
- lastname
id
location
```

How I actually parsed the string was by creating a class with a single exposed static method, along with some private helper methods to recursively parse the string.

Produces the following output:

The API also returns 400 errors with an appropriate response header / message for incorrect input strings.
