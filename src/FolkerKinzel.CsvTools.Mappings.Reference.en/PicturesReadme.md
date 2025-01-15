# Bitmap graphics in SHFB documentation
- Put the picture in the `images` directory and set its build action to `Content` (Build action `Image` won't work.)
- Reference the picture in the XML comments like this:
```csharp
<img src="images\MultiColumnConverter.png"/>
```
The path **MUST NOT** start with the backslash (e.g., `\images\..`). Otherwise the links will not 
work with GitHub Pages because the root directory `github.io` is not accessible.

In order to overcome this issue, the website project will use a post build step to copy 
the `images` directory and all of its contents into the `html` directory of the newly created
website to make the created links work.