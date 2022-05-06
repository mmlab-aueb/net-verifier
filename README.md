# VC verifier
This a VC verifier implemented by the [ZeroTrustVC](https://mm.aueb.gr/projects/zerotrustvc) project. 
The verifier can be used as a [.net Authorization Handler](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-6.0).

## Usage

TBD



## Testing

### Prerequisites
Tests are executed using pytest and pytest-asyncio. To install it execute: 

```bash
python3 -m pip install  pytest 
python3 -m pip install pytest-asyncio
python3 -m pip install requests
```

### Running the tests
From the root directory run `python3 -m pytest -s  tests/` For shorter output alternatively you can run `python3 -m pytest tests/ -s --tb=short`

