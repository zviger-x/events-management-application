#!/bin/bash

# Запускаем MongoDB в режиме реплики
mongod --replSet rs0 --bind_ip_all &

# Ждём, пока MongoDB поднимется
sleep 5

# Инициализируем реплику
mongosh --host localhost:27017 --eval '
rs.initiate({
  _id: "rs0",
  members: [{ _id: 0, host: "MongoServer:27017" }]
});
'
# Не даём контейнеру завершиться
tail -f /dev/null