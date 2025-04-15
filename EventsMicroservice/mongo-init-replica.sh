#!/bin/bash
REPLICA_SET=${MONGO_REPLICA_SET:-rs0}
HOST=${MONGO_HOST:-localhost}
PORT=${MONGO_PORT:-27017}
KEYFILE=${MONGO_KEYFILE:-"defaultkey123"}
ADMIN_USERNAME=${MONGO_INITDB_ROOT_USERNAME:-root}
ADMIN_PASSWORD=${MONGO_INITDB_ROOT_PASSWORD:-rootpassword}
PIDFILE="/tmp/mongo.pid"

# �����
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # ��� �����

echo -e "${BLUE}LOG: Starting MongoDB with replica: $REPLICA_SET. Host: $HOST:$PORT${NC}"

# ���������� keyfile
echo -e "${GREEN}LOG: Preparing keyfile...${NC}"
echo "$KEYFILE" > /etc/mongo-keyfile
chmod 600 /etc/mongo-keyfile

# ������ mongod ��� �����������
echo -e "${GREEN}LOG: Launching mongod without auth for initial setup...${NC}"
mongod --replSet "$REPLICA_SET" \
  --bind_ip_all \
  --keyFile /etc/mongo-keyfile \
  --logpath /var/log/mongod.log \
  --fork \
  --pidfilepath "$PIDFILE"

# ��������� �������, ���� MongoDB ��������� ����������
sleep 5

# ������������� �������-���� � ��������� ������ �����
echo -e "${BLUE}LOG: Initiating replica set...${NC}"
mongosh --host "localhost:$PORT" --eval "
try {
  rs.initiate({
    _id: '$REPLICA_SET',
    members: [{ _id: 0, host: '$HOST:$PORT' }]
  });
} catch (e) { print(e); }
"

# ���, ���� PRIMARY �������
echo -e "${GREEN}LOG: Waiting for PRIMARY...${NC}"
until mongosh --host "localhost:$PORT" --quiet --eval "rs.isMaster().ismaster" | grep true; do
  sleep 1
done

# �������� ��������������
echo -e "${GREEN}LOG: Creating admin user...${NC}"
mongosh --host "localhost:$PORT" --eval "
db.getSiblingDB('admin').createUser({
  user: '$ADMIN_USERNAME',
  pwd: '$ADMIN_PASSWORD',
  roles: [ { role: 'root', db: 'admin' } ]
});
"

# ��������� mongod �������
echo -e "${GREEN}LOG: Shutting down mongod after setup via PID...${NC}"
if [ -f "$PIDFILE" ]; then
  kill -15 "$(cat $PIDFILE)"
  rm "$PIDFILE"
else
  echo -e "${RED}LOG: PID file not found, skipping shutdown${NC}"
fi

# ��������� ����������
sleep 5

# ���������� mongod � ������������
echo -e "${BLUE}LOG: Restarting mongod with --auth...${NC}"
exec mongod --replSet "$REPLICA_SET" --bind_ip_all --auth --keyFile /etc/mongo-keyfile