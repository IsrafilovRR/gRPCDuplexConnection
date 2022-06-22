-- create db
DROP DATABASE IF EXISTS ozon;
CREATE DATABASE ozon;

-- create table   
DROP TABLE IF EXISTS orders;
CREATE TABLE orders
(
    id           BIGSERIAL
        CONSTRAINT pk_orders_id PRIMARY KEY,
    creationDate timestamp      NOT NULL DEFAULT current_timestamp,
    clientId     BIGINT         NOT NULL,
    isCompleted  BOOLEAN        NOT NULL,
    orderTypeId  BIGINT         NOT NULL,
    warehouseId  BIGINT         NOT NULL,
    amount       DECIMAL(18, 2) NOT NULL,
    itemsData    json           NOT NULL
);
-- create index on orders table
CREATE INDEX orders_status_creation_date ON orders (isCompleted, creationDate);

-- trigger function
CREATE OR REPLACE FUNCTION orders_insert_trigger_function()
    RETURNS TRIGGER AS
$$
DECLARE
    current_warehouseId  bigint;
    partition_table_name TEXT;
BEGIN
    current_warehouseId = NEW.warehouseId;
    partition_table_name := FORMAT('orders_%s', NEW.warehouseId::TEXT);
    IF (TO_REGCLASS(partition_table_name::TEXT) ISNULL) THEN
        EXECUTE FORMAT(
                'CREATE TABLE %I ('
                    '  CHECK (warehouseId = %L)'
                    ') INHERITS (orders);'
            , partition_table_name, current_warehouseId);
        EXECUTE FORMAT(
                'ALTER TABLE ONLY %1$I ADD CONSTRAINT %1$s_pk PRIMARY KEY (id);'
            , partition_table_name);
        EXECUTE FORMAT(
                'CREATE INDEX %1$s_status_creation_date ON %1$I (isCompleted, creationDate);'
            , partition_table_name);
    END IF;
    EXECUTE FORMAT('INSERT INTO %I VALUES ($1.*)', partition_table_name) USING NEW;

    RETURN NULL;
END;
$$
    LANGUAGE plpgsql;

-- create trigger for orders table
CREATE TRIGGER insert_orders_trigger
    BEFORE INSERT
    ON orders
    FOR EACH ROW
EXECUTE FUNCTION orders_insert_trigger_function();

-- helper function to get random from range
CREATE OR REPLACE FUNCTION random_between(low INT, high INT)
    RETURNS INT AS
$$
BEGIN
    RETURN floor(random() * (high - low + 1) + low);
END;
$$ language 'plpgsql' STRICT;

-- fill data
INSERT INTO orders(clientId, isCompleted, warehouseId, amount, orderTypeId, itemsData)
SELECT random_between(1, 50),
       (CASE
            WHEN floor(random() * 10 + 1)::int > 4 THEN true
            ELSE false
           END) AS is_completed,
       random_between(1, 30),
       (random() * 10000)::decimal(18, 2),
       1,
       '[
         {
           "Id": 7,
           "Count": 7
         },
         {
           "Id": 23,
           "Count": 13
         }
       ]'::json
FROM generate_series(1, 5000);
  