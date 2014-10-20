-- Table: base_parameters
INSERT INTO base_parameters (name, str_value)
VALUES 	('product_name', 'CupboardDesigner'), 
	('version', '1.1'), 
	('edition', 'gpl');

-- Table: orders
ALTER TABLE orders ADD COLUMN total_price NUMERIC DEFAULT ( 0 );
ALTER TABLE orders ADD COLUMN price_correction INTEGER DEFAULT ( 0 );
ALTER TABLE orders ADD COLUMN cutting_base BOOLEAN NOT NULL DEFAULT ( 0 );
ALTER TABLE orders ADD COLUMN basis_price NUMERIC DEFAULT ( 0 );

-- Table: nomenclature
ALTER TABLE nomenclature ADD COLUMN price NUMERIC DEFAULT ( 0 );
ALTER TABLE nomenclature ADD COLUMN price_type TEXT;

-- Table: basis
ALTER TABLE basis ADD COLUMN width INTEGER DEFAULT ( 0 );

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
    ordinal     INTEGER,
    image       BLOB,
    image_size  INTEGER,
    description VARCHAR,
    width       INTEGER        NOT NULL
                               DEFAULT '1',
    height      INTEGER        NOT NULL
                               DEFAULT '1' 
);
INSERT INTO cubes (id, name, ordinal, image, image_size, description, width, height)
	SELECT id, name, ordinal, image, image_size, description, 
		CASE WHEN lenght is null THEN 1 ELSE lenght / 400 END as width, 
		CASE WHEN height is null THEN 1 ELSE height / 400 END as height
	FROM nomenclature
	WHERE type = 'cube';

-- Table: order_cubes_details
CREATE TABLE order_cubes_details ( 
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id        INTEGER REFERENCES orders ( id ) ON DELETE CASCADE,
    cube_id         INTEGER REFERENCES cubes ( id ) ON DELETE CASCADE,
    nomenclature_id INTEGER,
    count           INTEGER,
    price           NUMERIC,
    comment         TEXT,
    discount        INTEGER DEFAULT ( 0 ) 
);

-- Table: order_basis_details
CREATE TABLE order_basis_details ( 
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id        INTEGER REFERENCES orders ( id ) ON DELETE CASCADE,
    nomenclature_id INTEGER,
    count           INTEGER,
    price           NUMERIC,
    comment         TEXT,
    discount        INTEGER DEFAULT ( 0 ),
    facing_id       INTEGER REFERENCES facing ( id ),
    material_id     INTEGER REFERENCES materials ( id )
);

-- Table: order_services
CREATE TABLE order_services ( 
    id       INTEGER,
    order_id INTEGER REFERENCES orders ( id ) ON DELETE CASCADE,
    name     TEXT,
    price    NUMERIC,
    discount INTEGER,
    comment  TEXT,
    PRIMARY KEY ( id ) 
);

-- Table: order_details
CREATE TABLE order_details ( 
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id    INTEGER REFERENCES orders ( id ) ON DELETE CASCADE,
    cube_id     INTEGER REFERENCES cubes ( id ),
    count       INTEGER,
    facing_id   INTEGER REFERENCES facing ( id ),
    material_id INTEGER REFERENCES materials ( id ),
    comment     TEXT,
    price       NUMERIC DEFAULT ( 0 ) 
);

INSERT INTO order_details 
(order_id, cube_id, count, facing_id, material_id, comment, price) 
SELECT order_components.order_id, cubes.id, order_components.count, order_components.facing_id, order_components.material_id, order_components.comment, 0 as price
FROM order_components 
LEFT JOIN nomenclature ON nomenclature.id = order_components.nomenclature_id
LEFT JOIN cubes ON cubes.name = nomenclature.name
WHERE nomenclature.type = 'cube';

INSERT INTO order_basis_details
(order_id, nomenclature_id, facing_id, material_id, count, price, comment, discount)
SELECT order_components.order_id, order_components.nomenclature_id, order_components.facing_id, order_components.material_id, order_components.count, 0 as price, order_components.comment, 0 as discount
FROM order_components 
LEFT JOIN nomenclature ON nomenclature.id = order_components.nomenclature_id
LEFT JOIN orders ON orders.id = order_components.order_id
WHERE nomenclature.type = 'construct';

DELETE FROM order_details WHERE order_id NOT IN (
SELECT id FROM orders);

DELETE FROM order_basis_details WHERE order_id NOT IN (
SELECT id FROM orders);

DROP TABLE order_components;
DELETE FROM nomenclature WHERE type = 'cube';

-- Вставляем новые кубы.
    INSERT INTO [cubes] ( [name], [ordinal], [image], [image_size], [description], [width], [height]) VALUES ('Ниша 3х3', 38, '<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg
   xmlns:ooo="http://xml.openoffice.org/svg/export"
   xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:cc="http://creativecommons.org/ns#"
   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
   xmlns:svg="http://www.w3.org/2000/svg"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   version="1.2"
   width="583.59052"
   height="583.94489"
   viewBox="0 0 16470.221 16480.223"
   preserveAspectRatio="xMidYMid"
   clip-path="url(#presentation_clip_path)"
   xml:space="preserve"
   id="svg2"
   inkscape:version="0.48.5 r10040"
   sodipodi:docname="3x3.svg"
   style="fill-rule:evenodd;stroke-width:28.22200012;stroke-linejoin:round"><metadata
     id="metadata105"><rdf:RDF><cc:Work
         rdf:about=""><dc:format>image/svg+xml</dc:format><dc:type
           rdf:resource="http://purl.org/dc/dcmitype/StillImage" /></cc:Work></rdf:RDF></metadata><sodipodi:namedview
     pagecolor="#ffffff"
     bordercolor="#666666"
     borderopacity="1"
     objecttolerance="10"
     gridtolerance="10"
     guidetolerance="10"
     inkscape:pageopacity="0"
     inkscape:pageshadow="2"
     inkscape:window-width="640"
     inkscape:window-height="480"
     id="namedview103"
     showgrid="false"
     fit-margin-top="0"
     fit-margin-left="0"
     fit-margin-right="0"
     fit-margin-bottom="0"
     inkscape:zoom="0.22425739"
     inkscape:cx="296.57873"
     inkscape:cy="294.31101"
     inkscape:window-x="286"
     inkscape:window-y="80"
     inkscape:window-maximized="0"
     inkscape:current-layer="svg2" /><defs
     class="ClipPathGroup"
     id="defs4"><clipPath
       id="presentation_clip_path"
       clipPathUnits="userSpaceOnUse"><rect
         x="0"
         y="0"
         width="21000"
         height="29700"
         id="rect7" /></clipPath></defs><defs
     class="TextShapeIndex"
     id="defs9"><g
       ooo:slide="id1"
       ooo:id-list="id3"
       id="g11" /></defs><defs
     class="EmbeddedBulletChars"
     id="defs13"><g
       id="bullet-char-template(57356)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 580,1141 1163,571 580,0 -4,571 580,1141 z"
         id="path16"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(57354)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 8,1128 1129,0 L 1137,0 8,0 8,1128 z"
         id="path19"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10146)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 174,0 602,739 174,1481 1456,739 174,0 z m 1184,739 -1049,607 350,-607 699,0 z"
         id="path22"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10132)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 2015,739 1276,0 717,0 l 543,543 -1086,0 0,393 1086,0 -543,545 557,0 741,-742 z"
         id="path25"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10007)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 0,-2 c -7,16 -16,29 -25,39 l 381,530 c -94,256 -141,385 -141,387 0,25 13,38 40,38 9,0 21,-2 34,-5 21,4 42,12 65,25 l 27,-13 111,-251 280,301 64,-25 24,25 c 21,-10 41,-24 62,-43 C 886,937 835,863 770,784 769,783 710,716 594,584 L 774,223 c 0,-27 -21,-55 -63,-84 l 16,-20 C 717,90 699,76 672,76 641,76 570,178 457,381 L 164,-76 c -22,-34 -53,-51 -92,-51 -42,0 -63,17 -64,51 -7,9 -10,24 -10,44 0,9 1,19 2,30 z"
         id="path28"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10004)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 285,-33 C 182,-33 111,30 74,156 52,228 41,333 41,471 c 0,78 14,145 41,201 34,71 87,106 158,106 53,0 88,-31 106,-94 l 23,-176 c 8,-64 28,-97 59,-98 l 735,706 c 11,11 33,17 66,17 42,0 63,-15 63,-46 l 0,-122 c 0,-36 -10,-64 -30,-84 L 442,47 C 390,-6 338,-33 285,-33 z"
         id="path31"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(9679)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 813,0 C 632,0 489,54 383,161 276,268 223,411 223,592 c 0,181 53,324 160,431 106,107 249,161 430,161 179,0 323,-54 432,-161 108,-107 162,-251 162,-431 0,-180 -54,-324 -162,-431 C 1136,54 992,0 813,0 z"
         id="path34"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8226)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 346,457 c -73,0 -137,26 -191,78 -54,51 -81,114 -81,188 0,73 27,136 81,188 54,52 118,78 191,78 73,0 134,-26 185,-79 51,-51 77,-114 77,-187 0,-75 -25,-137 -76,-188 -50,-52 -112,-78 -186,-78 z"
         id="path37"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8211)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m -4,459 1139,0 0,147 -1139,0 0,-147 z"
         id="path40"
         inkscape:connector-curvature="0" /></g></defs><defs
     class="TextEmbeddedBitmaps"
     id="defs42" /><g
     id="g44"
     transform="translate(-2129.8891,-6675.889)"><g
       id="id2"
       class="Master_Slide"><g
         id="bg-id2"
         class="Background" /><g
         id="bo-id2"
         class="BackgroundObjects" /></g></g><g
     class="SlideGroup"
     id="g49"
     transform="translate(-2129.8891,-6675.889)"><g
       id="g51"><g
         id="id1"
         class="Slide"
         clip-path="url(#presentation_clip_path)"><g
           class="Page"
           id="g54"><g
             class="Graphic"
             id="g56"><g
               id="id3"><line
                 x1="2386"
                 y1="6930"
                 x2="2144"
                 y2="6690"
                 id="line59"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2144"
                 y1="23142"
                 x2="2144"
                 y2="6690"
                 id="line61"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2144"
                 y1="23142"
                 x2="2386"
                 y2="22902"
                 id="line63"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2386"
                 y1="22902"
                 x2="2386"
                 y2="6930"
                 id="line65"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2144"
                 y1="6690"
                 x2="18586"
                 y2="6690"
                 id="line67"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18371"
                 y1="6930"
                 x2="18586"
                 y2="6690"
                 id="line69"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2386"
                 y1="6930"
                 x2="2144"
                 y2="6690"
                 id="line71"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2386"
                 y1="6930"
                 x2="18371"
                 y2="6930"
                 id="line73"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18586"
                 y1="23142"
                 x2="18371"
                 y2="22902"
                 id="line75"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18586"
                 y1="23142"
                 x2="18586"
                 y2="6690"
                 id="line77"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18371"
                 y1="6930"
                 x2="18586"
                 y2="6690"
                 id="line79"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18371"
                 y1="6930"
                 x2="18371"
                 y2="22902"
                 id="line81"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18586"
                 y1="23142"
                 x2="18371"
                 y2="22902"
                 id="line83"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2386"
                 y1="22902"
                 x2="18371"
                 y2="22902"
                 id="line85"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2144"
                 y1="23142"
                 x2="2386"
                 y2="22902"
                 id="line87"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="2144"
                 y1="23142"
                 x2="18586"
                 y2="23142"
                 id="line89"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4862"
                 y1="6930"
                 x2="4889"
                 y2="6930"
                 id="line91"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4889"
                 y1="6690"
                 x2="4916"
                 y2="6690"
                 id="line93"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4889"
                 y1="23142"
                 x2="4916"
                 y2="23142"
                 id="line95"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4862"
                 y1="22902"
                 x2="4889"
                 y2="22902"
                 id="line97"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18371"
                 y1="20400"
                 x2="18371"
                 y2="20373"
                 id="line99"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="18586"
                 y1="20426"
                 x2="18586"
                 y2="20400"
                 id="line101"
                 style="fill:#000000;stroke:#000000" /></g></g></g></g></g></g></svg>', 10214, null, 3, 3);
INSERT INTO [cubes] ([name], [ordinal], [image], [image_size], [description], [width], [height]) VALUES ('Ниша 3х2 горизонтальная', 39, '<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg
   xmlns:ooo="http://xml.openoffice.org/svg/export"
   xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:cc="http://creativecommons.org/ns#"
   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
   xmlns:svg="http://www.w3.org/2000/svg"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   version="1.2"
   width="666.92914"
   height="444.97635"
   viewBox="0 0 18822.222 12558.222"
   preserveAspectRatio="xMidYMid"
   clip-path="url(#presentation_clip_path)"
   xml:space="preserve"
   id="svg2"
   inkscape:version="0.48.5 r10040"
   sodipodi:docname="3x2.svg"
   style="fill-rule:evenodd;stroke-width:28.22200012;stroke-linejoin:round"><metadata
     id="metadata105"><rdf:RDF><cc:Work
         rdf:about=""><dc:format>image/svg+xml</dc:format><dc:type
           rdf:resource="http://purl.org/dc/dcmitype/StillImage" /></cc:Work></rdf:RDF></metadata><sodipodi:namedview
     pagecolor="#ffffff"
     bordercolor="#666666"
     borderopacity="1"
     objecttolerance="10"
     gridtolerance="10"
     guidetolerance="10"
     inkscape:pageopacity="0"
     inkscape:pageshadow="2"
     inkscape:window-width="640"
     inkscape:window-height="480"
     id="namedview103"
     showgrid="false"
     fit-margin-top="0"
     fit-margin-left="0"
     fit-margin-right="0"
     fit-margin-bottom="0"
     inkscape:zoom="0.22425739"
     inkscape:cx="337.11416"
     inkscape:cy="224.25983"
     inkscape:window-x="154"
     inkscape:window-y="80"
     inkscape:window-maximized="0"
     inkscape:current-layer="svg2" /><defs
     class="ClipPathGroup"
     id="defs4"><clipPath
       id="presentation_clip_path"
       clipPathUnits="userSpaceOnUse"><rect
         x="0"
         y="0"
         width="21000"
         height="29700"
         id="rect7" /></clipPath></defs><defs
     class="TextShapeIndex"
     id="defs9"><g
       ooo:slide="id1"
       ooo:id-list="id3"
       id="g11" /></defs><defs
     class="EmbeddedBulletChars"
     id="defs13"><g
       id="bullet-char-template(57356)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 580,1141 1163,571 580,0 -4,571 580,1141 z"
         id="path16"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(57354)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 8,1128 1129,0 L 1137,0 8,0 8,1128 z"
         id="path19"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10146)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 174,0 602,739 174,1481 1456,739 174,0 z m 1184,739 -1049,607 350,-607 699,0 z"
         id="path22"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10132)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 2015,739 1276,0 717,0 l 543,543 -1086,0 0,393 1086,0 -543,545 557,0 741,-742 z"
         id="path25"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10007)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 0,-2 c -7,16 -16,29 -25,39 l 381,530 c -94,256 -141,385 -141,387 0,25 13,38 40,38 9,0 21,-2 34,-5 21,4 42,12 65,25 l 27,-13 111,-251 280,301 64,-25 24,25 c 21,-10 41,-24 62,-43 C 886,937 835,863 770,784 769,783 710,716 594,584 L 774,223 c 0,-27 -21,-55 -63,-84 l 16,-20 C 717,90 699,76 672,76 641,76 570,178 457,381 L 164,-76 c -22,-34 -53,-51 -92,-51 -42,0 -63,17 -64,51 -7,9 -10,24 -10,44 0,9 1,19 2,30 z"
         id="path28"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10004)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 285,-33 C 182,-33 111,30 74,156 52,228 41,333 41,471 c 0,78 14,145 41,201 34,71 87,106 158,106 53,0 88,-31 106,-94 l 23,-176 c 8,-64 28,-97 59,-98 l 735,706 c 11,11 33,17 66,17 42,0 63,-15 63,-46 l 0,-122 c 0,-36 -10,-64 -30,-84 L 442,47 C 390,-6 338,-33 285,-33 z"
         id="path31"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(9679)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 813,0 C 632,0 489,54 383,161 276,268 223,411 223,592 c 0,181 53,324 160,431 106,107 249,161 430,161 179,0 323,-54 432,-161 108,-107 162,-251 162,-431 0,-180 -54,-324 -162,-431 C 1136,54 992,0 813,0 z"
         id="path34"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8226)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 346,457 c -73,0 -137,26 -191,78 -54,51 -81,114 -81,188 0,73 27,136 81,188 54,52 118,78 191,78 73,0 134,-26 185,-79 51,-51 77,-114 77,-187 0,-75 -25,-137 -76,-188 -50,-52 -112,-78 -186,-78 z"
         id="path37"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8211)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m -4,459 1139,0 0,147 -1139,0 0,-147 z"
         id="path40"
         inkscape:connector-curvature="0" /></g></defs><defs
     class="TextEmbeddedBitmaps"
     id="defs42" /><g
     id="g44"
     transform="translate(-985.88914,-8620.889)"><g
       id="id2"
       class="Master_Slide"><g
         id="bg-id2"
         class="Background" /><g
         id="bo-id2"
         class="BackgroundObjects" /></g></g><g
     class="SlideGroup"
     id="g49"
     transform="translate(-985.88914,-8620.889)"><g
       id="g51"><g
         id="id1"
         class="Slide"
         clip-path="url(#presentation_clip_path)"><g
           class="Page"
           id="g54"><g
             class="Graphic"
             id="g56"><g
               id="id3"><line
                 x1="1267"
                 y1="8898"
                 x2="1000"
                 y2="8635"
                 id="line59"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21165"
                 x2="1000"
                 y2="8635"
                 id="line61"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21165"
                 x2="1267"
                 y2="20901"
                 id="line63"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1267"
                 y1="20901"
                 x2="1267"
                 y2="8898"
                 id="line65"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="8635"
                 x2="19794"
                 y2="8635"
                 id="line67"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19528"
                 y1="8898"
                 x2="19794"
                 y2="8635"
                 id="line69"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1267"
                 y1="8898"
                 x2="1000"
                 y2="8635"
                 id="line71"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1267"
                 y1="8898"
                 x2="19528"
                 y2="8898"
                 id="line73"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19794"
                 y1="21165"
                 x2="19528"
                 y2="20901"
                 id="line75"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19794"
                 y1="21165"
                 x2="19794"
                 y2="8635"
                 id="line77"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19528"
                 y1="8898"
                 x2="19794"
                 y2="8635"
                 id="line79"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19528"
                 y1="8898"
                 x2="19528"
                 y2="20901"
                 id="line81"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19794"
                 y1="21165"
                 x2="19528"
                 y2="20901"
                 id="line83"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1267"
                 y1="20901"
                 x2="19528"
                 y2="20901"
                 id="line85"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21165"
                 x2="1267"
                 y2="20901"
                 id="line87"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21165"
                 x2="19794"
                 y2="21165"
                 id="line89"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4095"
                 y1="8898"
                 x2="4136"
                 y2="8898"
                 id="line91"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4136"
                 y1="8635"
                 x2="4177"
                 y2="8635"
                 id="line93"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4136"
                 y1="21165"
                 x2="4177"
                 y2="21165"
                 id="line95"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4095"
                 y1="20901"
                 x2="4115"
                 y2="20901"
                 id="line97"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19528"
                 y1="18022"
                 x2="19528"
                 y2="18002"
                 id="line99"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19794"
                 y1="18063"
                 x2="19794"
                 y2="18042"
                 id="line101"
                 style="fill:#000000;stroke:#000000" /></g></g></g></g></g></g></svg>', 10214, null, 3, 2);
INSERT INTO [cubes] ([name], [ordinal], [image], [image_size], [description], [width], [height]) VALUES ('Ниша 3х2 вертикальная', 40, '<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg
   xmlns:ooo="http://xml.openoffice.org/svg/export"
   xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:cc="http://creativecommons.org/ns#"
   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
   xmlns:svg="http://www.w3.org/2000/svg"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   version="1.2"
   width="444.97635"
   height="666.92914"
   viewBox="0 0 12558.221 18822.223"
   preserveAspectRatio="xMidYMid"
   clip-path="url(#presentation_clip_path)"
   xml:space="preserve"
   id="svg2"
   inkscape:version="0.48.5 r10040"
   sodipodi:docname="3x2 (Ð²ÐµÑ).svg"
   style="fill-rule:evenodd;stroke-width:28.22200012;stroke-linejoin:round"><metadata
     id="metadata105"><rdf:RDF><cc:Work
         rdf:about=""><dc:format>image/svg+xml</dc:format><dc:type
           rdf:resource="http://purl.org/dc/dcmitype/StillImage" /></cc:Work></rdf:RDF></metadata><sodipodi:namedview
     pagecolor="#ffffff"
     bordercolor="#666666"
     borderopacity="1"
     objecttolerance="10"
     gridtolerance="10"
     guidetolerance="10"
     inkscape:pageopacity="0"
     inkscape:pageshadow="2"
     inkscape:window-width="640"
     inkscape:window-height="480"
     id="namedview103"
     showgrid="false"
     fit-margin-top="0"
     fit-margin-left="0"
     fit-margin-right="0"
     fit-margin-bottom="0"
     inkscape:zoom="0.22425739"
     inkscape:cx="220.71653"
     inkscape:cy="337.11416"
     inkscape:window-x="239"
     inkscape:window-y="70"
     inkscape:window-maximized="0"
     inkscape:current-layer="svg2" /><defs
     class="ClipPathGroup"
     id="defs4"><clipPath
       id="presentation_clip_path"
       clipPathUnits="userSpaceOnUse"><rect
         x="0"
         y="0"
         width="21000"
         height="29700"
         id="rect7" /></clipPath></defs><defs
     class="TextShapeIndex"
     id="defs9"><g
       ooo:slide="id1"
       ooo:id-list="id3"
       id="g11" /></defs><defs
     class="EmbeddedBulletChars"
     id="defs13"><g
       id="bullet-char-template(57356)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 580,1141 1163,571 580,0 -4,571 580,1141 z"
         id="path16"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(57354)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 8,1128 1129,0 L 1137,0 8,0 8,1128 z"
         id="path19"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10146)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 174,0 602,739 174,1481 1456,739 174,0 z m 1184,739 -1049,607 350,-607 699,0 z"
         id="path22"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10132)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 2015,739 1276,0 717,0 l 543,543 -1086,0 0,393 1086,0 -543,545 557,0 741,-742 z"
         id="path25"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10007)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 0,-2 c -7,16 -16,29 -25,39 l 381,530 c -94,256 -141,385 -141,387 0,25 13,38 40,38 9,0 21,-2 34,-5 21,4 42,12 65,25 l 27,-13 111,-251 280,301 64,-25 24,25 c 21,-10 41,-24 62,-43 C 886,937 835,863 770,784 769,783 710,716 594,584 L 774,223 c 0,-27 -21,-55 -63,-84 l 16,-20 C 717,90 699,76 672,76 641,76 570,178 457,381 L 164,-76 c -22,-34 -53,-51 -92,-51 -42,0 -63,17 -64,51 -7,9 -10,24 -10,44 0,9 1,19 2,30 z"
         id="path28"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10004)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 285,-33 C 182,-33 111,30 74,156 52,228 41,333 41,471 c 0,78 14,145 41,201 34,71 87,106 158,106 53,0 88,-31 106,-94 l 23,-176 c 8,-64 28,-97 59,-98 l 735,706 c 11,11 33,17 66,17 42,0 63,-15 63,-46 l 0,-122 c 0,-36 -10,-64 -30,-84 L 442,47 C 390,-6 338,-33 285,-33 z"
         id="path31"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(9679)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 813,0 C 632,0 489,54 383,161 276,268 223,411 223,592 c 0,181 53,324 160,431 106,107 249,161 430,161 179,0 323,-54 432,-161 108,-107 162,-251 162,-431 0,-180 -54,-324 -162,-431 C 1136,54 992,0 813,0 z"
         id="path34"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8226)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 346,457 c -73,0 -137,26 -191,78 -54,51 -81,114 -81,188 0,73 27,136 81,188 54,52 118,78 191,78 73,0 134,-26 185,-79 51,-51 77,-114 77,-187 0,-75 -25,-137 -76,-188 -50,-52 -112,-78 -186,-78 z"
         id="path37"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8211)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m -4,459 1139,0 0,147 -1139,0 0,-147 z"
         id="path40"
         inkscape:connector-curvature="0" /></g></defs><defs
     class="TextEmbeddedBitmaps"
     id="defs42" /><g
     id="g44"
     transform="translate(-4270.8891,-5541.889)"><g
       id="id2"
       class="Master_Slide"><g
         id="bg-id2"
         class="Background" /><g
         id="bo-id2"
         class="BackgroundObjects" /></g></g><g
     class="SlideGroup"
     id="g49"
     transform="translate(-4270.8891,-5541.889)"><g
       id="g51"><g
         id="id1"
         class="Slide"
         clip-path="url(#presentation_clip_path)"><g
           class="Page"
           id="g54"><g
             class="Graphic"
             id="g56"><g
               id="id3"><line
                 x1="4548"
                 y1="24083"
                 x2="4285"
                 y2="24350"
                 id="line59"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="24350"
                 x2="4285"
                 y2="24350"
                 id="line61"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="24350"
                 x2="16551"
                 y2="24083"
                 id="line63"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16551"
                 y1="24083"
                 x2="4548"
                 y2="24083"
                 id="line65"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4285"
                 y1="24350"
                 x2="4285"
                 y2="5556"
                 id="line67"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4548"
                 y1="5822"
                 x2="4285"
                 y2="5556"
                 id="line69"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4548"
                 y1="24083"
                 x2="4285"
                 y2="24350"
                 id="line71"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4548"
                 y1="24083"
                 x2="4548"
                 y2="5822"
                 id="line73"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="5556"
                 x2="16551"
                 y2="5822"
                 id="line75"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="5556"
                 x2="4285"
                 y2="5556"
                 id="line77"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4548"
                 y1="5822"
                 x2="4285"
                 y2="5556"
                 id="line79"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4548"
                 y1="5822"
                 x2="16551"
                 y2="5822"
                 id="line81"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="5556"
                 x2="16551"
                 y2="5822"
                 id="line83"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16551"
                 y1="24083"
                 x2="16551"
                 y2="5822"
                 id="line85"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="24350"
                 x2="16551"
                 y2="24083"
                 id="line87"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="24350"
                 x2="16815"
                 y2="5556"
                 id="line89"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4548"
                 y1="21255"
                 x2="4548"
                 y2="21214"
                 id="line91"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="4285"
                 y1="21214"
                 x2="4285"
                 y2="21173"
                 id="line93"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16815"
                 y1="21214"
                 x2="16815"
                 y2="21173"
                 id="line95"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="16551"
                 y1="21255"
                 x2="16551"
                 y2="21235"
                 id="line97"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="13672"
                 y1="5822"
                 x2="13652"
                 y2="5822"
                 id="line99"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="13713"
                 y1="5556"
                 x2="13692"
                 y2="5556"
                 id="line101"
                 style="fill:#000000;stroke:#000000" /></g></g></g></g></g></g></svg>', 10227, null, 2, 3);
INSERT INTO [cubes] ([name], [ordinal], [image], [image_size], [description], [width], [height]) VALUES ('Ниша 4х3 горизонтальная', 41, '<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg
   xmlns:ooo="http://xml.openoffice.org/svg/export"
   xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:cc="http://creativecommons.org/ns#"
   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
   xmlns:svg="http://www.w3.org/2000/svg"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   version="1.2"
   width="666.00787"
   height="499.89761"
   viewBox="0 0 18796.222 14108.222"
   preserveAspectRatio="xMidYMid"
   clip-path="url(#presentation_clip_path)"
   xml:space="preserve"
   id="svg2"
   inkscape:version="0.48.5 r10040"
   sodipodi:docname="4x3.svg"
   style="fill-rule:evenodd;stroke-width:28.22200012;stroke-linejoin:round"><metadata
     id="metadata105"><rdf:RDF><cc:Work
         rdf:about=""><dc:format>image/svg+xml</dc:format><dc:type
           rdf:resource="http://purl.org/dc/dcmitype/StillImage" /></cc:Work></rdf:RDF></metadata><sodipodi:namedview
     pagecolor="#ffffff"
     bordercolor="#666666"
     borderopacity="1"
     objecttolerance="10"
     gridtolerance="10"
     guidetolerance="10"
     inkscape:pageopacity="0"
     inkscape:pageshadow="2"
     inkscape:window-width="640"
     inkscape:window-height="480"
     id="namedview103"
     showgrid="false"
     fit-margin-top="0"
     fit-margin-left="0"
     fit-margin-right="0"
     fit-margin-bottom="0"
     inkscape:zoom="0.22425739"
     inkscape:cx="337.11416"
     inkscape:cy="251.93306"
     inkscape:window-x="98"
     inkscape:window-y="113"
     inkscape:window-maximized="0"
     inkscape:current-layer="svg2" /><defs
     class="ClipPathGroup"
     id="defs4"><clipPath
       id="presentation_clip_path"
       clipPathUnits="userSpaceOnUse"><rect
         x="0"
         y="0"
         width="21000"
         height="29700"
         id="rect7" /></clipPath></defs><defs
     class="TextShapeIndex"
     id="defs9"><g
       ooo:slide="id1"
       ooo:id-list="id3"
       id="g11" /></defs><defs
     class="EmbeddedBulletChars"
     id="defs13"><g
       id="bullet-char-template(57356)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 580,1141 1163,571 580,0 -4,571 580,1141 z"
         id="path16"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(57354)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 8,1128 1129,0 L 1137,0 8,0 8,1128 z"
         id="path19"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10146)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 174,0 602,739 174,1481 1456,739 174,0 z m 1184,739 -1049,607 350,-607 699,0 z"
         id="path22"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10132)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 2015,739 1276,0 717,0 l 543,543 -1086,0 0,393 1086,0 -543,545 557,0 741,-742 z"
         id="path25"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10007)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 0,-2 c -7,16 -16,29 -25,39 l 381,530 c -94,256 -141,385 -141,387 0,25 13,38 40,38 9,0 21,-2 34,-5 21,4 42,12 65,25 l 27,-13 111,-251 280,301 64,-25 24,25 c 21,-10 41,-24 62,-43 C 886,937 835,863 770,784 769,783 710,716 594,584 L 774,223 c 0,-27 -21,-55 -63,-84 l 16,-20 C 717,90 699,76 672,76 641,76 570,178 457,381 L 164,-76 c -22,-34 -53,-51 -92,-51 -42,0 -63,17 -64,51 -7,9 -10,24 -10,44 0,9 1,19 2,30 z"
         id="path28"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10004)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 285,-33 C 182,-33 111,30 74,156 52,228 41,333 41,471 c 0,78 14,145 41,201 34,71 87,106 158,106 53,0 88,-31 106,-94 l 23,-176 c 8,-64 28,-97 59,-98 l 735,706 c 11,11 33,17 66,17 42,0 63,-15 63,-46 l 0,-122 c 0,-36 -10,-64 -30,-84 L 442,47 C 390,-6 338,-33 285,-33 z"
         id="path31"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(9679)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 813,0 C 632,0 489,54 383,161 276,268 223,411 223,592 c 0,181 53,324 160,431 106,107 249,161 430,161 179,0 323,-54 432,-161 108,-107 162,-251 162,-431 0,-180 -54,-324 -162,-431 C 1136,54 992,0 813,0 z"
         id="path34"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8226)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 346,457 c -73,0 -137,26 -191,78 -54,51 -81,114 -81,188 0,73 27,136 81,188 54,52 118,78 191,78 73,0 134,-26 185,-79 51,-51 77,-114 77,-187 0,-75 -25,-137 -76,-188 -50,-52 -112,-78 -186,-78 z"
         id="path37"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8211)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m -4,459 1139,0 0,147 -1139,0 0,-147 z"
         id="path40"
         inkscape:connector-curvature="0" /></g></defs><defs
     class="TextEmbeddedBitmaps"
     id="defs42" /><g
     id="g44"
     transform="translate(-985.88914,-7851.889)"><g
       id="id2"
       class="Master_Slide"><g
         id="bg-id2"
         class="Background" /><g
         id="bo-id2"
         class="BackgroundObjects" /></g></g><g
     class="SlideGroup"
     id="g49"
     transform="translate(-985.88914,-7851.889)"><g
       id="g51"><g
         id="id1"
         class="Slide"
         clip-path="url(#presentation_clip_path)"><g
           class="Page"
           id="g54"><g
             class="Graphic"
             id="g56"><g
               id="id3"><line
                 x1="1207"
                 y1="8072"
                 x2="1000"
                 y2="7866"
                 id="line59"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21946"
                 x2="1000"
                 y2="7866"
                 id="line61"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21946"
                 x2="1207"
                 y2="21740"
                 id="line63"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1207"
                 y1="21740"
                 x2="1207"
                 y2="8072"
                 id="line65"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="7866"
                 x2="19768"
                 y2="7866"
                 id="line67"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19561"
                 y1="8072"
                 x2="19768"
                 y2="7866"
                 id="line69"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1207"
                 y1="8072"
                 x2="1000"
                 y2="7866"
                 id="line71"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1207"
                 y1="8072"
                 x2="19561"
                 y2="8072"
                 id="line73"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19768"
                 y1="21946"
                 x2="19561"
                 y2="21740"
                 id="line75"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19768"
                 y1="21946"
                 x2="19768"
                 y2="7866"
                 id="line77"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19561"
                 y1="8072"
                 x2="19768"
                 y2="7866"
                 id="line79"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19561"
                 y1="8072"
                 x2="19561"
                 y2="21740"
                 id="line81"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19768"
                 y1="21946"
                 x2="19561"
                 y2="21740"
                 id="line83"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1207"
                 y1="21740"
                 x2="19561"
                 y2="21740"
                 id="line85"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21946"
                 x2="1207"
                 y2="21740"
                 id="line87"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="1000"
                 y1="21946"
                 x2="19768"
                 y2="21946"
                 id="line89"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3326"
                 y1="8072"
                 x2="3349"
                 y2="8072"
                 id="line91"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3349"
                 y1="7866"
                 x2="3372"
                 y2="7866"
                 id="line93"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3349"
                 y1="21946"
                 x2="3372"
                 y2="21946"
                 id="line95"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3326"
                 y1="21740"
                 x2="3349"
                 y2="21740"
                 id="line97"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19561"
                 y1="19599"
                 x2="19561"
                 y2="19576"
                 id="line99"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="19768"
                 y1="19621"
                 x2="19768"
                 y2="19599"
                 id="line101"
                 style="fill:#000000;stroke:#000000" /></g></g></g></g></g></g></svg>', 10214, null, 4, 3);
INSERT INTO [cubes] ([name], [ordinal], [image], [image_size], [description], [width], [height]) VALUES ('Ниша 4х3 вертикальная', 42, '<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg
   xmlns:ooo="http://xml.openoffice.org/svg/export"
   xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:cc="http://creativecommons.org/ns#"
   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
   xmlns:svg="http://www.w3.org/2000/svg"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   version="1.2"
   width="499.89761"
   height="666.00787"
   viewBox="0 0 14108.222 18796.223"
   preserveAspectRatio="xMidYMid"
   clip-path="url(#presentation_clip_path)"
   xml:space="preserve"
   id="svg2"
   inkscape:version="0.48.5 r10040"
   sodipodi:docname="4x3 Ð²ÐµÑ.svg"
   style="fill-rule:evenodd;stroke-width:28.22200012;stroke-linejoin:round"><metadata
     id="metadata105"><rdf:RDF><cc:Work
         rdf:about=""><dc:format>image/svg+xml</dc:format><dc:type
           rdf:resource="http://purl.org/dc/dcmitype/StillImage" /></cc:Work></rdf:RDF></metadata><sodipodi:namedview
     pagecolor="#ffffff"
     bordercolor="#666666"
     borderopacity="1"
     objecttolerance="10"
     gridtolerance="10"
     guidetolerance="10"
     inkscape:pageopacity="0"
     inkscape:pageshadow="2"
     inkscape:window-width="640"
     inkscape:window-height="480"
     id="namedview103"
     showgrid="false"
     fit-margin-top="0"
     fit-margin-left="0"
     fit-margin-right="0"
     fit-margin-bottom="0"
     inkscape:zoom="0.22425739"
     inkscape:cx="247.96456"
     inkscape:cy="337.11416"
     inkscape:window-x="0"
     inkscape:window-y="27"
     inkscape:window-maximized="0"
     inkscape:current-layer="svg2" /><defs
     class="ClipPathGroup"
     id="defs4"><clipPath
       id="presentation_clip_path"
       clipPathUnits="userSpaceOnUse"><rect
         x="0"
         y="0"
         width="21000"
         height="29700"
         id="rect7" /></clipPath></defs><defs
     class="TextShapeIndex"
     id="defs9"><g
       ooo:slide="id1"
       ooo:id-list="id3"
       id="g11" /></defs><defs
     class="EmbeddedBulletChars"
     id="defs13"><g
       id="bullet-char-template(57356)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 580,1141 1163,571 580,0 -4,571 580,1141 z"
         id="path16"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(57354)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 8,1128 1129,0 L 1137,0 8,0 8,1128 z"
         id="path19"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10146)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 174,0 602,739 174,1481 1456,739 174,0 z m 1184,739 -1049,607 350,-607 699,0 z"
         id="path22"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10132)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 2015,739 1276,0 717,0 l 543,543 -1086,0 0,393 1086,0 -543,545 557,0 741,-742 z"
         id="path25"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10007)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 0,-2 c -7,16 -16,29 -25,39 l 381,530 c -94,256 -141,385 -141,387 0,25 13,38 40,38 9,0 21,-2 34,-5 21,4 42,12 65,25 l 27,-13 111,-251 280,301 64,-25 24,25 c 21,-10 41,-24 62,-43 C 886,937 835,863 770,784 769,783 710,716 594,584 L 774,223 c 0,-27 -21,-55 -63,-84 l 16,-20 C 717,90 699,76 672,76 641,76 570,178 457,381 L 164,-76 c -22,-34 -53,-51 -92,-51 -42,0 -63,17 -64,51 -7,9 -10,24 -10,44 0,9 1,19 2,30 z"
         id="path28"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(10004)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 285,-33 C 182,-33 111,30 74,156 52,228 41,333 41,471 c 0,78 14,145 41,201 34,71 87,106 158,106 53,0 88,-31 106,-94 l 23,-176 c 8,-64 28,-97 59,-98 l 735,706 c 11,11 33,17 66,17 42,0 63,-15 63,-46 l 0,-122 c 0,-36 -10,-64 -30,-84 L 442,47 C 390,-6 338,-33 285,-33 z"
         id="path31"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(9679)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="M 813,0 C 632,0 489,54 383,161 276,268 223,411 223,592 c 0,181 53,324 160,431 106,107 249,161 430,161 179,0 323,-54 432,-161 108,-107 162,-251 162,-431 0,-180 -54,-324 -162,-431 C 1136,54 992,0 813,0 z"
         id="path34"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8226)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m 346,457 c -73,0 -137,26 -191,78 -54,51 -81,114 -81,188 0,73 27,136 81,188 54,52 118,78 191,78 73,0 134,-26 185,-79 51,-51 77,-114 77,-187 0,-75 -25,-137 -76,-188 -50,-52 -112,-78 -186,-78 z"
         id="path37"
         inkscape:connector-curvature="0" /></g><g
       id="bullet-char-template(8211)"
       transform="scale(4.8828125e-4,-4.8828125e-4)"><path
         d="m -4,459 1139,0 0,147 -1139,0 0,-147 z"
         id="path40"
         inkscape:connector-curvature="0" /></g></defs><defs
     class="TextEmbeddedBitmaps"
     id="defs42" /><g
     id="g44"
     transform="translate(-3501.8891,-5567.889)"><g
       id="id2"
       class="Master_Slide"><g
         id="bg-id2"
         class="Background" /><g
         id="bo-id2"
         class="BackgroundObjects" /></g></g><g
     class="SlideGroup"
     id="g49"
     transform="translate(-3501.8891,-5567.889)"><g
       id="g51"><g
         id="id1"
         class="Slide"
         clip-path="url(#presentation_clip_path)"><g
           class="Page"
           id="g54"><g
             class="Graphic"
             id="g56"><g
               id="id3"><line
                 x1="3722"
                 y1="24143"
                 x2="3516"
                 y2="24350"
                 id="line59"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="24350"
                 x2="3516"
                 y2="24350"
                 id="line61"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="24350"
                 x2="17390"
                 y2="24143"
                 id="line63"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17390"
                 y1="24143"
                 x2="3722"
                 y2="24143"
                 id="line65"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3516"
                 y1="24350"
                 x2="3516"
                 y2="5582"
                 id="line67"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3722"
                 y1="5789"
                 x2="3516"
                 y2="5582"
                 id="line69"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3722"
                 y1="24143"
                 x2="3516"
                 y2="24350"
                 id="line71"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3722"
                 y1="24143"
                 x2="3722"
                 y2="5789"
                 id="line73"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="5582"
                 x2="17390"
                 y2="5789"
                 id="line75"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="5582"
                 x2="3516"
                 y2="5582"
                 id="line77"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3722"
                 y1="5789"
                 x2="3516"
                 y2="5582"
                 id="line79"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3722"
                 y1="5789"
                 x2="17390"
                 y2="5789"
                 id="line81"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="5582"
                 x2="17390"
                 y2="5789"
                 id="line83"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17390"
                 y1="24143"
                 x2="17390"
                 y2="5789"
                 id="line85"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="24350"
                 x2="17390"
                 y2="24143"
                 id="line87"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="24350"
                 x2="17596"
                 y2="5582"
                 id="line89"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3722"
                 y1="22024"
                 x2="3722"
                 y2="22001"
                 id="line91"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="3516"
                 y1="22001"
                 x2="3516"
                 y2="21978"
                 id="line93"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17596"
                 y1="22001"
                 x2="17596"
                 y2="21978"
                 id="line95"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="17390"
                 y1="22024"
                 x2="17390"
                 y2="22001"
                 id="line97"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="15249"
                 y1="5789"
                 x2="15226"
                 y2="5789"
                 id="line99"
                 style="fill:#000000;stroke:#000000" /><line
                 x1="15271"
                 y1="5582"
                 x2="15249"
                 y2="5582"
                 id="line101"
                 style="fill:#000000;stroke:#000000" /></g></g></g></g></g></g></svg>', 10223, null, 3, 4);

-- Добавляем новую номенклатуру
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (50, 'cube', 50, null, 'Куб 1', null, null, null, null, null, null, null, null, 6000, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (51, 'cube', 51, null, 'Куб 2', null, null, null, null, null, null, null, null, 10800, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (52, 'cube', 52, null, 'Куб 3', null, null, null, null, null, null, null, null, 15600, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (53, 'cube', 53, null, 'Куб 4', null, null, null, null, null, null, null, null, 20400, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (54, 'cube', 54, null, 'Ящики 1 (для куба 1)', null, null, null, null, null, null, null, null, 12000, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (55, 'cube', 55, null, 'Ящики 2 (для куба 1)', null, null, null, null, null, null, null, null, 14000, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (56, 'cube', 56, null, 'Ящики 3 (для куба 2)', null, null, null, null, null, null, null, null, 18000, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (57, 'cube', 57, null, 'Фасад 1 (на 1 куб)', null, null, null, null, null, 'стекло или филёнка', null, null, 4500, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (58, 'cube', 58, null, 'Фасад 2 (на 2 куба)', null, null, null, null, null, 'стекло или филёнка', null, null, 7500, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (59, 'cube', 59, null, 'Фасад 3 (на 3 куба)', null, null, null, null, null, 'стекло или филёнка', null, null, 10500, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (60, 'cube', 60, null, 'Фасад 4 (на 4 куба)', null, null, null, null, null, 'С полкой горизонтально стекло или ДСП шпонированное на 2 куба', null, null, 13500, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (61, 'cube', 61, null, 'Фасад 5 (на 4 куба)', null, null, null, null, null, 'Рекомендуется для комодов', null, null, 15000, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (62, 'cube', 62, null, 'Вставка 1 (на 1 куб)', null, null, null, null, null, null, null, null, 550, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (63, 'cube', 63, null, 'Вставка 2 (на 2 куба)', null, null, null, null, null, null, null, null, 1100, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (64, 'cube', 64, null, 'Вставка 3 (на 1 куб)', null, null, null, null, null, null, null, null, 1400, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (65, 'cube', 65, null, 'Вставка 4 (на 1 куб)', null, null, null, null, null, 'Стекло  прозрачное 6 мм', null, null, 400, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (66, 'cube', 66, null, 'Вставка 5 (на 2 куба)', null, null, null, null, null, 'Стекло  прозрачное 6 мм', null, null, 800, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (67, 'construct', 67, null, 'Бок 40мм под запил (гориз.)', null, null, 0, null, 0, 'Боковой каркас 40мм под запил 45 градусов для встроенных шкафов, обвязка по периметру', null, null, 3752, 'width');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (68, 'cube', 68, null, 'Задняя стенка', null, null, null, null, null, '4 мм для кубов более 4*4', null, null, 430, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (69, 'construct', 69, null, 'Освещение 1 лампа', null, null, 0, null, 0, 'в ком-те блок питания + сумматор', null, null, 5200, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (70, 'construct', 70, null, 'Освещение 2 лампы', null, null, 0, null, 0, 'в ком-те блок питания + сумматор', null, null, 6290, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (71, 'construct', 71, null, 'Освещение 3 лампы', null, null, 0, null, 0, 'в ком-те блок питания + сумматор', null, null, 7380, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (72, 'construct', 72, null, 'Освещение 4 лампы', null, null, 0, null, 0, 'в ком-те блок питания + сумматор', null, null, 8470, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (73, 'construct', 73, null, 'Дополнительно 1', null, null, 0, null, 0, null, null, null, null, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (74, 'construct', 74, null, 'Дополнительно 2', null, null, 0, null, 0, null, null, null, null, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (75, 'cube', 75, null, 'Бок под 45 горизонт.', null, null, null, null, null, 'Боковой каркас 40мм под запил 45 градусов  для встроенных шкафов , обвязка по периметру', null, null, 3752, 'width');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (76, 'cube', 76, null, 'Бок под 45 верт.', null, null, null, null, null, 'Боковой каркас 40мм под запил 45 градусов  для встроенных шкафов , обвязка по периметру', null, null, 3752, 'height');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (77, 'cube', 77, null, 'Задняя стенка 3х3', null, null, null, null, null, null, null, null, 3870, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (78, 'cube', 78, null, 'Задняя стенка 3х2', null, null, null, null, null, null, null, null, 2580, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (79, 'cube', 79, null, 'Задняя стенка 4х3', null, null, null, null, null, null, null, null, 5160, 'none');
INSERT INTO [nomenclature] ([id], [type], [ordinal], [article], [name], [widht], [height], [plush], [lenght], [plusl], [description], [image], [image_size], [price], [price_type]) VALUES (80, 'construct', 67, null, 'Бок 40мм под запил (верт.)', null, null, 0, null, 0, 'Боковой каркас 40мм под запил 45 градусов  для встроенных шкафов , обвязка по периметру', null, null, 3752, 'height');
  
-- Добавляем ширину к каркасам
UPDATE basis SET width = 400 WHERE id = 1 ;
UPDATE basis SET width = 424 WHERE id = 3 ;
UPDATE basis SET width = 424 WHERE id = 4 ;
UPDATE basis SET width = 424, delta_h = 232 WHERE id = 5 ;
UPDATE basis SET width = 424 WHERE id = 6 ;
UPDATE basis SET width = 424 WHERE id = 8 ;
UPDATE basis SET width = 400 WHERE id = 10 ;

-- Исправляем название фигурного наличника
UPDATE nomenclature SET name = 'Фигурный наличник (шт.)' WHERE id = 47;

-- Добавляем цены к старой номенклатуре
UPDATE nomenclature SET description = 'Толщина 40 мм', price = 4500, price_type = 'width' WHERE id = 37 ;
UPDATE nomenclature SET description = 'Под покраску', price = 9365, price_type = 'width' WHERE id = 38 ;
UPDATE nomenclature SET description = 'Под покраску', price = 5620, price_type = 'width' WHERE id = 39 ;
UPDATE nomenclature SET description = 'Основа под портал 42 мм', price = 2000, price_type = 'width' WHERE id = 40 ;
UPDATE nomenclature SET description = '', price = 2680, price_type = 'height' WHERE id = 41 ;
UPDATE nomenclature SET description = '', price = 3300, price_type = 'width' WHERE id = 43 ;
UPDATE nomenclature SET description = '', price = 3300, price_type = 'width' WHERE id = 44 ;
UPDATE nomenclature SET description = '', price = 1500, price_type = 'width' WHERE id = 45 ;
UPDATE nomenclature SET description = '', price = 1500, price_type = 'width' WHERE id = 46 ;
UPDATE nomenclature SET description = '', price = 1720, price_type = 'none' WHERE id = 47 ;
UPDATE nomenclature SET description = '', price = 6000, price_type = 'none' WHERE id = 48 ;

-- Вносим комплектацию кубов
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (1, 18, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (2, 19, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (3, 19, 62, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (4, 20, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (5, 20, 62, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (6, 21, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (7, 21, 64, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (8, 22, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (9, 22, 57, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (10, 23, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (11, 23, 57, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (12, 24, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (13, 24, 54, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (14, 25, 50, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (15, 25, 55, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (16, 49, 51, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (17, 49, 56, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (18, 26, 51, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (19, 27, 51, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (20, 28, 51, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (21, 28, 58, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (22, 28, 62, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (23, 29, 51, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (24, 29, 58, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (25, 29, 65, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (26, 30, 52, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (27, 30, 59, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (28, 30, 62, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (29, 31, 52, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (30, 31, 59, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (31, 31, 65, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (32, 32, 53, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (33, 33, 53, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (34, 33, 60, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (35, 33, 63, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (36, 34, 53, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (37, 34, 60, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (38, 34, 66, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (39, 35, 53, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (40, 35, 61, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (41, 35, 63, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (42, 36, 53, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (43, 36, 61, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (44, 36, 66, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (45, 50, 75, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (46, 50, 76, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (47, 50, 77, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (48, 51, 75, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (49, 51, 76, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (50, 51, 78, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (51, 52, 75, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (52, 52, 76, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (53, 52, 78, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (54, 53, 75, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (55, 53, 76, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (56, 53, 79, 1);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (57, 54, 75, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (58, 54, 76, 2);
INSERT INTO [cubes_items] ([id], [cubes_id], [item_id], [count]) VALUES (59, 54, 79, 1);

-- Adding lights and 'additional' to basis

INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (46, 1, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (47, 1, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (48, 1, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (49, 1, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (50, 1, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (51, 1, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (52, 3, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (53, 3, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (54, 3, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (55, 3, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (56, 3, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (57, 3, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (58, 8, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (59, 8, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (60, 8, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (61, 8, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (62, 8, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (63, 8, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (64, 4, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (65, 4, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (66, 4, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (67, 4, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (68, 4, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (69, 4, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (70, 5, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (71, 5, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (72, 5, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (73, 5, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (74, 5, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (75, 5, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (76, 6, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (77, 6, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (78, 6, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (79, 6, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (80, 6, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (81, 6, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (82, 10, 69, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (83, 10, 70, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (84, 10, 71, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (85, 10, 72, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (86, 10, 73, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (87, 10, 74, 0);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (88, 10, 67, 2);
INSERT INTO [basis_items] ([id], [basis_id], [item_id], [count]) VALUES (89, 10, 80, 2);
DELETE FROM [basis_items] WHERE [id] = 43;