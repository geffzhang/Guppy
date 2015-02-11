# Guppy

Guppy is a tiny ASP.NET web site that serves up CommonMark (or HTML) content from an external directory.

It was written because I wanted to serve up CommonMark content from a GitHub repository,
but I wanted to host it in IIS so it could co-exist better with other IIS sites on the server.

## Getting Started

Getting Guppy up an running shouldn't take longer than a few minutes:

- Clone the repo
- Open Guppy.sln in Visual Studio 2013.
- Modify the GuppyContentDirectory key in web.config to point to the ExampleContent directory.
  - Note that this needs to be an absolute path, like:
    - C:\Repos\Guppy\ExampleContent
- Run the solution (F5) and point your browser at the VS web server.

## Creating Content

For an example of Guppy in action, the [Pinta website repo][2] powers the [Pinta web site][3].

#### Structure

There are 2 directories that are used to serve the website:

- **Content** - Contains content that is served
- **Theme** - Contains the skin (template, css, images, etc.) that presents the content

#### Technical Details

Content is preferred to be written in [CommonMark][1] (a Markdown derivitive), but can also
be written in HTML if advanced features are needed.

New URLs are created simply by adding a new .md or .html file to the Content directory.

For example, to create **example.com/new-page**, add a file called
***new-page.md*** or ***new-page.html*** to the Content directory.  

You can also create hierarchy by adding directories to the Content directory.  

For example, adding **Content/AwesomePages/new-page.md** will create **example.com/awesomepages/new-page**.

##### Content Order of Precedence

Given a URL like **example.com/releases**, content will be searched for in the following order:

- Content/releases.md
- Content/releases.html
- Content/Releases/index.md
- Content/Releases/index.html
- 404 (Not Found) returned

[1]: http://commonmark.org/
[2]: https://github.com/PintaProject/website
[3]: http://pinta-project.com
