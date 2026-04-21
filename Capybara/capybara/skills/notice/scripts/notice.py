import requests
import sys

url = "http://localhost:8081/api"

if len(sys.argv) > 1:
    for i, arg in enumerate(sys.argv[1:], 1):
        payload=arg.encode('utf-8')
        headers = {
            'User-Agent': 'Apifox/1.0.0 (https://apifox.com)'
        }
        response = requests.request("POST", url, headers=headers, data=payload)
        print(response.content.decode('utf-8', errors='ignore'));
        break;
        



