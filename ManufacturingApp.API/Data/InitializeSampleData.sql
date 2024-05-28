-- Insertion of Sample Data
-- https://github.com/zepeda-luis/ManufacturingApp

-- Insert Suppliers
INSERT INTO Supplier (Name, Description)
VALUES 
('ABC Chemicals', 'Description for ABC Chemicals'),
('XYZ Corporation', 'Description for XYZ Corporation');

-- Insert Raw Materials
INSERT INTO RawMaterial (Name, Description)
VALUES 
('Catalyst', 'Description for Catalyst'),
('Ethylene Oxide', 'Description for Ethylene Oxide'),
('Lauryl Alcohol', 'Description for Lauryl Alcohol');

-- Insert SupplierRawMaterial
INSERT INTO SupplierRawMaterial (SupplierId, RawMaterialId, Price)
VALUES 
((SELECT Id FROM Supplier WHERE Name = 'ABC Chemicals'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Catalyst'), 10),
((SELECT Id FROM Supplier WHERE Name = 'ABC Chemicals'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Ethylene Oxide'), 20),
((SELECT Id FROM Supplier WHERE Name = 'ABC Chemicals'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Lauryl Alcohol'), 15),
((SELECT Id FROM Supplier WHERE Name = 'XYZ Corporation'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Catalyst'), 12),
((SELECT Id FROM Supplier WHERE Name = 'XYZ Corporation'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Ethylene Oxide'), 18),
((SELECT Id FROM Supplier WHERE Name = 'XYZ Corporation'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Lauryl Alcohol'), 17);

-- Insert Recipe
INSERT INTO Recipe (Name, Description)
VALUES 
('Alcohol Ethoxylate Production', 'This recipe outlines the production process for Alcohol Ethoxylate, a common form of nonionic surfactant.');

-- Insert Products
INSERT INTO Product (Name, Description, SellingPrice)
VALUES 
('Alcohol Ethoxylate', 'A nonionic surfactant', 10.00),  
('Dioxane', 'A byproduct of the production process', 5.00);  

-- Insert RecipeRawMaterial
INSERT INTO RecipeRawMaterial (RecipeId, RawMaterialId, Quantity)
VALUES 
((SELECT Id FROM Recipe WHERE Name = 'Alcohol Ethoxylate Production'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Catalyst'), 1),
((SELECT Id FROM Recipe WHERE Name = 'Alcohol Ethoxylate Production'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Ethylene Oxide'), 5),
((SELECT Id FROM Recipe WHERE Name = 'Alcohol Ethoxylate Production'), 
 (SELECT Id FROM RawMaterial WHERE Name = 'Lauryl Alcohol'), 1);

-- Insert RecipeProduct
INSERT INTO RecipeProduct (RecipeId, ProductId, Quantity)
VALUES 
((SELECT Id FROM Recipe WHERE Name = 'Alcohol Ethoxylate Production'), 
 (SELECT Id FROM Product WHERE Name = 'Alcohol Ethoxylate'), 1),
((SELECT Id FROM Recipe WHERE Name = 'Alcohol Ethoxylate Production'), 
 (SELECT Id FROM Product WHERE Name = 'Dioxane'), 0.2);
