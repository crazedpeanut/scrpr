FROM node as build

COPY . .
RUN npm i
RUN npm run build

FROM nginx as publish

COPY --from=build build /usr/share/nginx/html