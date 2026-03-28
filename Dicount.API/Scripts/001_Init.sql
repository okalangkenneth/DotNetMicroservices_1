CREATE TABLE IF NOT EXISTS Coupon (
    Id      SERIAL PRIMARY KEY,
    ProductName     VARCHAR(100) NOT NULL,
    Description     TEXT,
    Amount          INT NOT NULL DEFAULT 0
);

INSERT INTO Coupon (ProductName, Description, Amount)
SELECT 'Apple iPhone 13', '10% off iPhone 13', 70
WHERE NOT EXISTS (SELECT 1 FROM Coupon WHERE ProductName = 'Apple iPhone 13');

INSERT INTO Coupon (ProductName, Description, Amount)
SELECT 'Samsung Galaxy S21', '15% off Galaxy S21', 120
WHERE NOT EXISTS (SELECT 1 FROM Coupon WHERE ProductName = 'Samsung Galaxy S21');

INSERT INTO Coupon (ProductName, Description, Amount)
SELECT 'Dell XPS 15', '5% off Dell XPS 15', 85
WHERE NOT EXISTS (SELECT 1 FROM Coupon WHERE ProductName = 'Dell XPS 15');

INSERT INTO Coupon (ProductName, Description, Amount)
SELECT 'Sony WH-1000XM4', '20% off Sony headphones', 70
WHERE NOT EXISTS (SELECT 1 FROM Coupon WHERE ProductName = 'Sony WH-1000XM4');
