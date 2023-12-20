WorkBC API Load Testing
===================

This folder contains tools to load-test the WorkBC API. It uses [`siege`](https://github.com/JoeDog/siege) as the 
load-testing tool.

It is recommended to run siege v4.1.6 or greater. This version
[includes a fix to show URLs that cause errors](https://github.com/JoeDog/siege/issues/216) - you will need to 
[download the source package and compile it yourself](https://download.joedog.org/siege/) if the packaged version is 
earlier. This is only needed to have more visibility into siege failures, not site errors (which are logged by siege 
in its verbose output).

# Installing siege locally
For testing the job board API, `--with-ssl` option is required
```shell
wget http://download.joedog.org/siege/siege-latest.tar.gz
tar -xvf siege-latest.tar.gz
cd siege-*
./configure --with-zlib --with-ssl --prefix=/usr/local
make
sudo make install
```

# Running siege locally targeting remote API
```shell
BASE_URL=<insert-host-here> ././load-test.sh
```

# Getting Started
```
cp .env.sample .env
# edit .env and provide the api root URL to test
 
docker-compose up
docker-compose exec test bash
./load-test.sh > siege.log 2>&1
```

Output looks like the following:
```
Running siege...
** SIEGE 4.1.6
** Preparing 2 concurrent users for battle.
The server is now under siege...
Lifting the server siege...
Transactions:		         311 hits
Availability:		      100.00 %
Elapsed time:		      121.01 secs
Data transferred:	       18.65 MB
Response time:		        0.29 secs
Transaction rate:	        2.57 trans/sec
Throughput:		        0.15 MB/sec
Concurrency:		        0.75
Successful transactions:         295
Failed transactions:	           0
Longest transaction:	        4.37
Shortest transaction:	        0.14
```

# Configuration
- `BASE_URL` environment variable targets a specific environment (for example: `http://localhost`)
- `siege.conf` controls the `siege` running parameters. These can be overridden during a run with 
- [command-line arguments](https://manpages.ubuntu.com/manpages/bionic/man1/siege.1.html).

# cases.txt file
The `cases.txt` file contains target URLs. Make sure each target is a relative URL starting in the following format:
```
$(PROTOCOL)$(HOST)/assets/icons/some-file
$(PROTOCOL)$(HOST)/some-end-point POST {"payload": "here"}
``` 


# Troubleshooting
Your testing machine will encounter socket errors as your ramp up the concurrent users. You need to tune your kernel parameters to accommodate higher numbers, e.g.
- https://askubuntu.com/questions/46339/how-could-i-tune-the-linux-kernel-parameters-so-that-socket-could-be-recycled-fr
- https://stackoverflow.com/questions/880557/socket-accept-too-many-open-files
