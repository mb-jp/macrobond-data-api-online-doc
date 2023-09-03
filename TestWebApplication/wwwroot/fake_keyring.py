import sys


class FakeKeyringBackend:
    @property
    def name(self) -> str:
        return "FakeKeyringBackend"


class FakeKeyringCredentials:
    def __init__(self, username: str, password: str):
        self.username = username
        self.password = password


class FakeKeyring:
    def get_keyring(self):
        return FakeKeyringBackend()

    def get_credential(self, service_name, username):
        return FakeKeyringCredentials("test", "test")


def patch():
    sys.modules["keyring"] = FakeKeyring()
