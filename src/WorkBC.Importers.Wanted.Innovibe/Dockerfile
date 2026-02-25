FROM php:8.3-cli-alpine AS build
RUN apk add --no-cache postgresql-dev autoconf gcc g++ make unzip \
    && docker-php-ext-install pdo_pgsql
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY src/ src/

FROM php:8.3-cli-alpine
RUN apk add --no-cache postgresql-libs \
    && apk add --no-cache --virtual .bd postgresql-dev \
    && docker-php-ext-install pdo_pgsql \
    && apk del .bd
WORKDIR /app
COPY --from=build /app/vendor vendor/
COPY --from=build /app/src src/
CMD ["php", "src/import.php"]
