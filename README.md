# .Net Canonicaljson


.Net library for producing JSON in canonical format as specified by [https://gibson042.github.io/canonicaljson-spec/](https://gibson042.github.io/canonicaljson-spec/). The provided interface matches that of native JSON object.

## Installation

Use the nuget package manager console to install canonicaljson.

```bash
Install-Package Stratumn.CanonicalJson
```

## Usage

```python
import Stratumn.CanonicalJson;

string obj = Canonicalizer.Canonizalize("{ \"a\": 12 }"));
```


## Development
Integration tests are located in [canonicaljson-spec](https://gibson042.github.io/canonicaljson-spec/)  .
