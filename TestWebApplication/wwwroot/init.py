import adapter
import fake
import fake_keyring

fake.patch()
adapter.patch()
fake_keyring.patch()
