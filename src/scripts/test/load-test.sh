#!/bin/bash

echo "HOST="$BASE_URL > urls.txt
cat cases.txt >> urls.txt

echo "Running siege..."
siege --rc=./siege.conf