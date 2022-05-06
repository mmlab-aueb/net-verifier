
import pytest
import requests
import json 
from issuer import Issuer


class TestJWT:

    def test_no_authorization_get(self):
        response  = requests.get("http://localhost:9000/secure/jwt-vc")
        assert(response.status_code == 401)
    
    def test_valid_authorization_get(self):
        token = Issuer().issue_valid_vc()
        headers = {'Authorization':'Bearer ' + token, 'Accept': 'application/json'}
        response  = requests.get("http://localhost:9000/secure/jwt-vc", headers = headers)
        print(response.text)
        assert(response.status_code == 200)

