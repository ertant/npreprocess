# npreprocess 

NPreprocess HTML, JavaScript, and other files with directives based off custom or environment configuration. 

Its ported from [preprocess](https://github.com/jsoverson/preprocess) to run with .Net framework.

## What does it look like?

```html
<head>
  <title>Your App

  <!-- @if NODE_ENV='production' -->
  <script src="some/production/lib/like/analytics.js"></script>
  <!-- @endif -->
  
</head>
<body>
  <!-- @ifdef DEBUG -->
  <h1>Debugging mode - <!-- @echo RELEASE_TAG --> </h1>
  <!-- @endif -->
  <p>
  <!-- @include welcome_message.txt -->
  </p>
</body>
```

```js 
var configValue = '/* @echo FOO */' || 'default value';

// @ifdef DEBUG 
someDebuggingCall()
// @endif

```

## Directive syntax

### Simple syntax

The most basic usage is for files that only have two states, non-processed and processed.
In this case, your `@exclude` directives are removed after preprocessing

```html
<body>
    <!-- @exclude -->
    <header>You're on dev!</header>
    <!-- @endexclude -->
</body>
```

After build

```html
<body>
</body>
```

### Advanced directives

 - `@if VAR='value'` / `@endif`
   This will include the enclosed block if your test passes
 - `@ifdef VAR` / `@endif`
   This will include the enclosed block if VAR is defined (typeof !== 'undefined')
 - `@ifndef VAR` / `@endif`
   This will include the enclosed block if VAR is not defined (typeof === 'undefined')
 - `@include`
   This will include the source from an external file
 - `@exclude` / `@endexclude`
   This will remove the enclosed block upon processing
 - `@echo VAR`
   This will include the environment variable VAR into your source

### Extended html Syntax

This is useful for more fine grained control of your files over multiple
environment configurations. You have access to simple tests of any variable within the context (or ENV, if not supplied)

```html
<body>
    <!-- @if NODE_ENV!='production' -->
    <header>You're on dev!</header>
    <!-- @endif -->

    <!-- @if NODE_ENV='production' -->
    <script src="some/production/javascript.js"></script>
    <!-- @endif -->

    <script>
    var fingerprint = '<!-- @echo COMMIT_HASH -->' || 'DEFAULT';
    </script>
</body>
```

With a `NODE_ENV` set to `production` and `0xDEADBEEF` in
`COMMIT_HASH` this will be built to look like

```html
<body>
    <script src="some/production/javascript.js"></script>

    <script>
    var fingerprint = '0xDEADBEEF' || 'DEFAULT';
    </script>
</body>
```

With NODE_ENV not set or set to dev and nothing in COMMIT_HASH,
the built file will be

```html
<body>
    <header>You're on dev!</header>

    <script>
    var fingerprint = '' || 'DEFAULT';
    </script>
</body>
```


### JavaScript, CSS, C, Java Syntax

Extended syntax below, but will work without specifying a test

```js
normalFunction();
//@exclude
superExpensiveDebugFunction()
//@endexclude

'/* @echo USERNAME */'

anotherFunction();
```

Built with a NODE_ENV of production :

```js
normalFunction();

'jsoverson'

anotherFunction();
```

CSS example

```css
body {
/* @if NODE_ENV=='development' */
  background-color: red;
/* @endif */

}
// @include util.css
```

(CSS preprocessing supports single line comment style directives)


## Configuration and Usage

```html
npreprocess inputFile [/out:outputFile] [/type:fileType] [/v:name=value]
```

* Input file argument is required.
* If no output file is specified <inputFileName>.processed.<inputFileExtension> is generated.
* If no type specified than input file extension is used as type. Supported extensions are JS, HTML, HTM, XML, CSS, PHP, C, CPP, CS, JAVA


## Release History

 - 1.0.0 Initial release

## License

Written by Ertan Tike, Ported from https://github.com/jsoverson/preprocess/ (Jarrod Overson)

Licensed under the Apache 2.0 license.
