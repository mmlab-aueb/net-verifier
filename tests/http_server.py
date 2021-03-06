import http.server
import socketserver
from http import HTTPStatus


class Handler(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        self.send_response(HTTPStatus.OK)
        self.end_headers()
        self.wfile.write(b'Hello world')

    def do_PUT(self):
        self.send_response(HTTPStatus.OK)
        self.end_headers()
        self.wfile.write(b'Hello world from PUT')

    def do_POST(self):
        self.send_response(HTTPStatus.OK)
        self.end_headers()
        self.wfile.write(b'Hello world from POST')


print("\n * Protected resource on  http://localhost:8080/ ")
httpd = socketserver.TCPServer(('localhost', 8080), Handler)
httpd.serve_forever()