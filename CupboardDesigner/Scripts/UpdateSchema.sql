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
DELETE FROM nomenclature WHERE type = 'cube';

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
    discount        INTEGER DEFAULT ( 0 ) 
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

-- Here must be logic for transferring order to new schema.
-- DROP TABLE order_components