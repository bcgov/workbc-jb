FROM ubuntu:jammy

ENV TERM linux
ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update && apt-get install -y \
  wget \
  libxml2-utils \
  siege \
  && rm -rf /var/lib/apt/lists/*

WORKDIR /app

CMD ["/bin/bash"]
