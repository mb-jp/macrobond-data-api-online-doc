# how to build macrobond-data-api

u need to add to setup.py
```
"keyring>=23.11.0;latform_machine!='wasm32'",
"ijson>=3.1.4; platform_machine!='wasm32'",
```