# Simple.TextTemplates
**A library for generating text output from templates that contain embedded tags.**

**The main features of this library are:**
* Supports simple tag structures (either/or)
*   **"Hello ${FirstName}"** - TagStyle.StringBraces
*   **"Hello {{FirstName}}"** - TagStyle.Handlebars
* Tags embedded within tags to any depth
* Escaped tags $$, $} or \{ or \} respectively
* Easy to use
* Delegate/Lambda based value lookup
* Compiled templates for execution against single or multiple sets of data
* Templates sourced from char[], string, StringBuilder, Streams and Readers
* Object extenders for char[], string, StringBuilder, Streams and Readers

**To Do:**
- [x] Add Handlebars style tags
- [x] Add escape handling
- [ ] Recreate Unit Tests
