-- Table: base_parameters
INSERT INTO base_parameters (name, str_value)
VALUES 	('product_name', 'CupboardDesigner'), 
	('version', '1.1.0.0'), 
	('edition', 'gpl');

-- Table: orders
ALTER TABLE orders ADD COLUMN total_price NUMERIC DEFAULT ( 0 );
ALTER TABLE orders ADD COLUMN price_correction INTEGER DEFAULT ( 0 );
ALTER TABLE orders ADD COLUMN cutting_base BOOLEAN NOT NULL DEFAULT ( 0 );
ALTER TABLE orders ADD COLUMN basis_price NUMERIC DEFAULT ( 0 );

-- Table: nomenclature
ALTER TABLE nomenclature ADD COLUMN price NUMERIC DEFAULT ( 0 );

-- Table: cubes_items
CREATE TABLE cubes_items ( 
    id       INTEGER NOT NULL
                     UNIQUE,
    cubes_id INTEGER NOT NULL,
    item_id  INTEGER NOT NULL,
    count    INTEGER NOT NULL
                     DEFAULT '1',
    PRIMARY KEY ( id ) 
);

-- Table: cubes
CREATE TABLE cubes ( 
    id          INTEGER        PRIMARY KEY
                               NOT NULL
                               UNIQUE,
    name        VARCHAR( 50 )  NOT NULL
                               UNIQUE,
    ordinal     INTEGER        UNIQUE,
    image       BLOB           NOT NULL,
    image_size  INTEGER        NOT NULL,
    description VARCHAR,
    width       INTEGER        NOT NULL
                               DEFAULT '1',
    height      INTEGER        NOT NULL
                               DEFAULT '1' 
);
INSERT INTO cubes (name, ordinal, image, image_size, description, width, height)
	SELECT name, ordinal, image, image_size, description, 
		CASE WHEN widht is null THEN 1 ELSE widht / 400 END as width, 
		CASE WHEN height is null THEN 1 ELSE height / 400 END as height
	FROM nomenclature
	WHERE type = 'cube';

-- Table: order_cubes_details
CREATE TABLE order_cubes_details ( 
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id        INTEGER REFERENCES orders ( id ),
    cube_id         INTEGER REFERENCES cubes ( id ),
    nomenclature_id INTEGER REFERENCES nomenclature ( id ),
    count           INTEGER,
    price           NUMERIC,
    comment         TEXT,
    discount        INTEGER DEFAULT ( 0 ) 
);

-- Table: order_basis_details
CREATE TABLE order_basis_details ( 
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id        INTEGER REFERENCES orders ( id ),
    basis_id        INTEGER REFERENCES basis ( id ),
    nomenclature_id INTEGER REFERENCES nomenclature ( id ),
    count           INTEGER,
    price           NUMERIC,
    comment         TEXT,
    discount        INTEGER DEFAULT ( 0 ),
    facing_id       INTEGER REFERENCES facing ( id ),
    facing          TEXT    REFERENCES facing ( name ),
    material_id     INTEGER REFERENCES materials ( id ),
    material        TEXT    REFERENCES materials ( name )
);

-- Table: order_services
CREATE TABLE order_services ( 
    id       INTEGER,
    order_id INTEGER,
    name     TEXT,
    price    NUMERIC,
    discount INTEGER,
    comment  TEXT,
    PRIMARY KEY ( id ) 
);

-- Table: order_details
CREATE TABLE order_details ( 
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id    INTEGER REFERENCES orders ( id ),
    cube_id     INTEGER REFERENCES cubes ( id ),
    count       INTEGER,
    facing_id   INTEGER REFERENCES facing ( id ),
    facing      TEXT    REFERENCES facing ( name ),
    material_id INTEGER REFERENCES materials ( id ),
    material    TEXT    REFERENCES materials ( name ),
    comment     TEXT,
    price       NUMERIC DEFAULT ( 0 ) 
);

INSERT INTO order_details 
(order_id, cube_id, count, facing_id, facing, material_id, material, comment, price) 
SELECT order_components.order_id, cubes.id, order_components.count, order_components.facing_id, facing.name, order_components.material_id, materials.name, order_components.comment, 0 as price
FROM order_components 
LEFT JOIN nomenclature ON nomenclature.id = order_components.nomenclature_id
LEFT JOIN cubes ON cubes.name = nomenclature.name
LEFT JOIN facing ON order_components.facing_id = facing.id
LEFT JOIN materials ON order_components.material_id = materials.id
WHERE nomenclature.type = 'cube';

INSERT INTO order_basis_details
(order_id, basis_id, nomenclature_id, count, price, comment, discount)
SELECT order_components.order_id, orders.basis_id, nomenclature.id, order_components.count, 0 as price, order_components.comment, 0 as discount
FROM order_components 
LEFT JOIN nomenclature ON nomenclature.id = order_components.id
LEFT JOIN orders ON orders.id = order_components.id
WHERE nomenclature.type = 'construct';

-- DELETE FROM nomenclature WHERE type = 'cube';
-- Here must be logic for transferring order to new schema.
-- DROP TABLE order_components