CREATE TABLE product.product (
    id serial PRIMARY KEY,
    productJson jsonb NOT NULL,
    created timestamp with time zone NOT NULL,
    updated timestamp with time zone NOT NULL
);

CREATE TABLE product.sampledata (
    executed timestamp with time zone NOT NULL
);
