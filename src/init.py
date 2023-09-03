import pyodide_http
import fake
import fake_keyring

pyodide_http.patch_all()
fake.patch()
fake_keyring.patch()
