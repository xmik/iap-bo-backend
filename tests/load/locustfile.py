from locust import HttpLocust, TaskSet, task
from requests_toolbelt.multipart.encoder import MultipartEncoder
import os
import json
import random

class MyTaskSet(TaskSet):
    min_wait = 5000
    max_wait = 15000

    def on_start(self):
        """ on_start is called when a Locust start before any task is scheduled """

    @task(1)
    def get_employees(self):
        response = self.client.get('/api/get_employees')
        print("Response status code:", response.status_code)
        if response.status_code != 200:
            response.failure("Got wrong status_code")
        assert response.status_code == 200


class WebsiteUser(HttpLocust):
    task_set = MyTaskSet
    min_wait = 100
    max_wait = 1000
