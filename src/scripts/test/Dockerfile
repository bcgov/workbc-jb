FROM ubuntu:jammy

ENV TERM linux
ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update && apt-get install -y \
      wget \
      zlib1g \
      zlib1g-dev \
      build-essential \
      libssl-dev \
    && wget http://download.joedog.org/siege/siege-latest.tar.gz \
    && tar xvf siege-latest.tar.gz \
    && cd siege-4.1.6 \
    && ./configure --with-zlib --with-ssl --prefix=/usr/local \
    && make \
    && make install

WORKDIR /app

COPY ./cases.txt /app
COPY ./siege.conf /app
COPY ./load-test.sh /app

ENV BASE_URL $BASE_URL

CMD ["BASE_URL=${BASE_URL}","/app/load-test.sh"]
