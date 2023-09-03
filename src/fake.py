import sys


class Fake:
    def __init__(self, name: str, error: str):
        self._fake_name_ = name
        self._error_ = error
        self.__spec__ = None
        sys.modules[name] = self

    def __getattr__(self, item):
        info = (
            self._error_
            if self._error_
            else "Fake module: " + self._fake_name_ + " attr: " + item
        )
        sys.tracebacklimit = 0
        raise Exception(info)


def patch():
    Fake("ijson", None)
    Fake("win32com", "Com is not supported")
    Fake("pywintypes", "Com is not supported")
