from urllib.parse import urlparse, urlunparse
import requests
from pyodide_http._requests import PyodideHTTPAdapter
from js import host


class TestHTTPAdapter(PyodideHTTPAdapter):
    def __init__(self):
        super().__init__()

    def get_proxy_url(self, url):
        url_obj = urlparse(url)
        url_list = list(url_obj)
        url_list[2] = "/proxy/" + url_list[1] + url_list[2]
        url_list[1] = host
        return urlunparse(url_list)

    def send(self, request, **kwargs):
        new_request = requests.Request(request.method, self.get_proxy_url(request.url))

        new_request.timeout = kwargs.get("timeout", 0)
        if not new_request.timeout:
            new_request.timeout = 0

        new_request.headers = dict(request.headers)

        prepared = new_request.prepare()
        prepared.body = request.body

        return super().send(prepared, **kwargs)


def patch():
    requests.sessions.Session._old_init = requests.sessions.Session.__init__

    def new_init(self):
        self._old_init()
        self.mount("https://", TestHTTPAdapter())
        self.mount("http://", TestHTTPAdapter())

    requests.sessions.Session._old_init = requests.sessions.Session.__init__
    requests.sessions.Session.__init__ = new_init
