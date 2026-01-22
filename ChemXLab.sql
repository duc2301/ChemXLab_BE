--TẠO DATABASE (Chạy riêng dòng này nếu cần, hoặc bỏ qua nếu đã có DB)
DROP DATABASE IF EXISTS "ChemXLab";
CREATE DATABASE "ChemXLab";

-- ==========================================
--TẠO EXTENSION & BẢNG
-- ==========================================
-- ==========================================
-- 1. KHỞI TẠO DATABASE
-- ==========================================
-- (Bỏ comment dòng dưới nếu chưa tạo DB)
-- DROP DATABASE IF EXISTS "ChemXLab";
-- CREATE DATABASE "ChemXLab";

-- ==========================================
-- 2. TẠO EXTENSION & DỌN DẸP
-- ==========================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Xóa bảng cũ để làm sạch (theo thứ tự khóa ngoại)
DROP TABLE IF EXISTS submissions, assignments, class_members, classes, reaction_components, reactions, chemicals, elements, subscriptions, packages, users CASCADE;

-- Xóa các Type Enum cũ (nếu còn sót lại)
DROP TYPE IF EXISTS user_role, component_role CASCADE;

-- ==========================================
-- 3. TẠO BẢNG (MODULES)
-- ==========================================

-- MODULE 1: AUTH & USERS
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100),
    -- [UPDATED] Dùng VARCHAR thay vì ENUM để dễ Scaffold
    -- Các giá trị gợi ý: 'ADMIN', 'TEACHER', 'STUDENT', 'ORG_MANAGER'
    role VARCHAR(50) DEFAULT 'STUDENT', 
    avatar_url TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- MODULE 2: CHEMISTRY CORE
CREATE TABLE elements (
    id SERIAL PRIMARY KEY,
    symbol VARCHAR(3) UNIQUE NOT NULL,
    name VARCHAR(50) NOT NULL,
    atomic_mass DECIMAL(10, 4),
    properties JSONB DEFAULT '{}' 
);

CREATE TABLE chemicals (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    formula VARCHAR(50) NOT NULL,
    common_name VARCHAR(255),
    iupac_name VARCHAR(255),
    state_at_room_temp VARCHAR(20),
    structure_3d_url TEXT,
    molecular_data JSONB, 
    is_public BOOLEAN DEFAULT TRUE,
    created_by UUID REFERENCES users(id)
);

CREATE TABLE reactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    description TEXT,
    conditions TEXT,
    is_reversible BOOLEAN DEFAULT FALSE,
    video_url TEXT,
    visual_config JSONB 
);

CREATE TABLE reaction_components (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    reaction_id UUID REFERENCES reactions(id) ON DELETE CASCADE,
    chemical_id UUID REFERENCES chemicals(id),
    -- Các giá trị gợi ý: 'REACTANT', 'PRODUCT', 'CATALYST'
    role VARCHAR(20) NOT NULL, 
    coefficient INT DEFAULT 1,
    state_in_reaction VARCHAR(20)
);

CREATE INDEX idx_reaction_chem ON reaction_components(chemical_id, role);

-- MODULE 3: LMS
CREATE TABLE classes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    teacher_id UUID REFERENCES users(id),
    name VARCHAR(100) NOT NULL,
    class_code VARCHAR(20) UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE class_members (
    class_id UUID REFERENCES classes(id),
    student_id UUID REFERENCES users(id),
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (class_id, student_id)
);

CREATE TABLE assignments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    class_id UUID REFERENCES classes(id),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    deadline TIMESTAMP WITH TIME ZONE,
    lab_config JSONB 
);

CREATE TABLE submissions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    assignment_id UUID REFERENCES assignments(id),
    student_id UUID REFERENCES users(id),
    score DECIMAL(5, 2),
    teacher_feedback TEXT,
    result_snapshot JSONB, 
    submitted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- MODULE 4: PAYMENT
CREATE TABLE packages (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50),
    price DECIMAL(12, 2),
    duration_days INT,
    features JSONB
);

CREATE TABLE subscriptions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    package_id INT REFERENCES packages(id),
    start_date TIMESTAMP,
    end_date TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE
);

-- ==========================================
-- 4. INSERT DATA (MẪU)
-- ==========================================

-- A. USERS
INSERT INTO users (email, password_hash, full_name, role) VALUES 
('admin@chemxlab.com', 'hashed_pass_1', 'System Admin', 'ADMIN'),
('student@chemxlab.com', 'hashed_pass_2', 'Nguyen Van A', 'STUDENT');

-- B. ELEMENTS (Bảng tuần hoàn 118 nguyên tố)
INSERT INTO elements (id, symbol, name, atomic_mass, properties) VALUES
(1, 'H', 'Hydrogen', 1.008, '{"group": 1, "period": 1, "category": "nonmetal", "color_hex": "#FFFFFF"}'),
(2, 'He', 'Helium', 4.0026, '{"group": 18, "period": 1, "category": "noble-gas", "color_hex": "#D9FFFF"}'),
-- ... (Các nguyên tố 3-117 giữ nguyên như cũ, tôi rút gọn để tiết kiệm chỗ hiển thị)
(118, 'Og', 'Oganesson', 294, '{"group": 18, "period": 7, "category": "noble-gas", "color_hex": "#EB0026"}');
-- (Bạn nhớ chạy lại phần insert elements đầy đủ của bạn ở đây nếu cần)

-- C. CHEMICALS (Hóa chất cơ bản + Nâng cao)
INSERT INTO chemicals (formula, common_name, state_at_room_temp, molecular_data, is_public) VALUES
-- 1. ĐƠN CHẤT
('H2', 'Hydrogen', 'GAS', '{"color_hex": "#FFFFFF", "flammable": true}', TRUE),
('O2', 'Oxygen', 'GAS', '{"color_hex": "#FFFFFF", "oxidizer": true}', TRUE),
('N2', 'Nitrogen', 'GAS', '{"color_hex": "#3050F8", "inert": true}', TRUE),
('Cl2', 'Chlorine', 'GAS', '{"color_hex": "#1FF01F", "toxic": true}', TRUE),
('Fe', 'Iron', 'SOLID', '{"color_hex": "#5c5c5c", "metallic": true}', TRUE),
('Cu', 'Copper', 'SOLID', '{"color_hex": "#b87333", "metallic": true}', TRUE),
('Zn', 'Zinc', 'SOLID', '{"color_hex": "#A4A4A4", "metallic": true}', TRUE),
('Al', 'Aluminium', 'SOLID', '{"color_hex": "#BFA6A6", "metallic": true}', TRUE),
('Mg', 'Magnesium', 'SOLID', '{"color_hex": "#8AFF00", "flammable_solid": true}', TRUE),
('Na', 'Sodium', 'SOLID', '{"color_hex": "#CCCCCC", "reactive_with_water": true}', TRUE),
('K', 'Potassium', 'SOLID', '{"color_hex": "#8F40D4", "reactive_with_water": true}', TRUE),
('S', 'Sulfur', 'SOLID', '{"color_hex": "#FFFF30", "powder": true}', TRUE),
('C', 'Carbon (Charcoal)', 'SOLID', '{"color_hex": "#000000"}', TRUE),

-- 2. AXIT
('HCl', 'Hydrochloric Acid', 'LIQUID', '{"concentration": "1M", "pH": 1}', TRUE),
('H2SO4', 'Sulfuric Acid', 'LIQUID', '{"viscosity": "high", "dehydrating": true}', TRUE),
('HNO3', 'Nitric Acid', 'LIQUID', '{"fuming": true, "oxidizer": true}', TRUE),
('CH3COOH', 'Acetic Acid (Vinegar)', 'LIQUID', '{"smell": "sour", "pH": 3}', TRUE),
('H3PO4', 'Phosphoric Acid', 'LIQUID', '{"viscosity": "medium"}', TRUE),

-- 3. BAZƠ
('NaOH', 'Sodium Hydroxide', 'SOLID', '{"form": "pellets", "deliquescent": true}', TRUE),
('KOH', 'Potassium Hydroxide', 'SOLID', '{"form": "pellets"}', TRUE),
('Ca(OH)2', 'Calcium Hydroxide (Limewater)', 'SOLID', '{"solubility": "low"}', TRUE),
('Ba(OH)2', 'Barium Hydroxide', 'SOLID', '{"toxic": true}', TRUE),
('NH3', 'Ammonia', 'GAS', '{"smell": "pungent", "soluble": true}', TRUE),

-- 4. MUỐI
('NaCl', 'Sodium Chloride', 'SOLID', '{"crystal": "cubic"}', TRUE),
('CuSO4', 'Copper(II) Sulfate', 'SOLID', '{"color_hex": "#1E90FF", "hydrate": true}', TRUE),
('CaCO3', 'Calcium Carbonate', 'SOLID', '{"color_hex": "#FFFFFF", "insoluble": true}', TRUE),
('AgNO3', 'Silver Nitrate', 'SOLID', '{"light_sensitive": true, "stains_skin": true}', TRUE),
('KMnO4', 'Potassium Permanganate', 'SOLID', '{"color_hex": "#800080", "oxidizer": true}', TRUE),
('KClO3', 'Potassium Chlorate', 'SOLID', '{"oxidizer": true, "explosive_potential": true}', TRUE),
('NH4NO3', 'Ammonium Nitrate', 'SOLID', '{"fertilizer": true}', TRUE),
('NaHCO3', 'Baking Soda', 'SOLID', '{"powder": true}', TRUE),
('BaCl2', 'Barium Chloride', 'SOLID', '{"toxic": true}', TRUE),
('FeCl3', 'Iron(III) Chloride', 'SOLID', '{"color_hex": "#FFA500", "deliquescent": true}', TRUE),

-- 5. OXIT
('CO2', 'Carbon Dioxide', 'GAS', '{"heavier_than_air": true}', TRUE),
('SO2', 'Sulfur Dioxide', 'GAS', '{"smell": "choking"}', TRUE),
('CaO', 'Calcium Oxide (Quicklime)', 'SOLID', '{"exothermic_with_water": true}', TRUE),
('Fe2O3', 'Iron(III) Oxide (Rust)', 'SOLID', '{"color_hex": "#8B0000"}', TRUE),
('CuO', 'Copper(II) Oxide', 'SOLID', '{"color_hex": "#000000"}', TRUE),
('MgO', 'Magnesium Oxide', 'SOLID', '{"color_hex": "#FFFFFF", "powder": true}', TRUE),
('H2O2', 'Hydrogen Peroxide', 'LIQUID', '{"oxidizer": true, "bleaching": true}', TRUE),

-- 6. HỮU CƠ (ĐÃ XÓA TRÙNG LẶP SUCROSE Ở CÁC PHẦN DƯỚI)
('CH4', 'Methane', 'GAS', '{"flammable": true, "fuel": true}', TRUE),
('C2H5OH', 'Ethanol', 'LIQUID', '{"flammable": true, "volatile": true}', TRUE),
('C6H12O6', 'Glucose', 'SOLID', '{"sweet": true}', TRUE),
('C12H22O11', 'Sucrose (Table Sugar)', 'SOLID', '{"crystal": "white"}', TRUE),
('C2H2', 'Acetylene', 'GAS', '{"flammable": true, "welding": true}', TRUE),

-- 7. KHÁC
('H2O', 'Water', 'LIQUID', '{"solvent": true, "pH": 7}', TRUE),

-- 8. HÓA CHẤT BỔ SUNG (ADVANCED PACK)
('Phenol', 'Phenolphthalein Solution', 'LIQUID', '{"color_hex": "#FFFFFF", "indicator": true, "ph_range": "8.2-10"}', TRUE),
('Methyl', 'Methyl Orange', 'LIQUID', '{"color_hex": "#FFA500", "indicator": true, "ph_range": "3.1-4.4"}', TRUE),
('Pb(NO3)2', 'Lead(II) Nitrate', 'SOLID', '{"color_hex": "#FFFFFF", "toxic": true}', TRUE),
('KI', 'Potassium Iodide', 'SOLID', '{"color_hex": "#FFFFFF"}', TRUE),
('PbI2', 'Lead(II) Iodide', 'SOLID', '{"color_hex": "#FFD700", "precipitate": true, "shiny": true}', TRUE),
('KNO3', 'Potassium Nitrate', 'SOLID', '{"color_hex": "#FFFFFF"}', TRUE),
('NH4Cl', 'Ammonium Chloride', 'SOLID', '{"color_hex": "#FFFFFF", "smoke": true}', TRUE),
('Fe(OH)3', 'Iron(III) Hydroxide', 'SOLID', '{"color_hex": "#8B4513", "precipitate": true}', TRUE),
('BaSO4', 'Barium Sulfate', 'SOLID', '{"color_hex": "#FFFFFF", "precipitate": true}', TRUE),

-- 9. HÓA CHẤT BỔ SUNG (MEGA PACK)
('CuCl2', 'Copper(II) Chloride', 'SOLID', '{"color_hex": "#008080", "crystal": "needle", "flame_color": "BLUE_GREEN"}', TRUE),
('CuCO3', 'Copper(II) Carbonate', 'SOLID', '{"color_hex": "#2E8B57", "powder": true}', TRUE),
('Cu(NO3)2', 'Copper(II) Nitrate', 'SOLID', '{"color_hex": "#1E90FF", "crystal": "blue"}', TRUE),
('Ag2O', 'Silver Oxide', 'SOLID', '{"color_hex": "#362622", "powder": true}', TRUE),
('Ag', 'Silver Metal', 'SOLID', '{"color_hex": "#C0C0C0", "shiny": true, "metallic": true}', TRUE),
('Al2O3', 'Aluminium Oxide', 'SOLID', '{"color_hex": "#FFFFFF", "hardness": "high"}', TRUE),
('AlCl3', 'Aluminium Chloride', 'SOLID', '{"color_hex": "#FFFFE0"}', TRUE),
('NaAlO2', 'Sodium Aluminate', 'SOLID', '{"color_hex": "#FFFFFF"}', TRUE),
('FeSO4', 'Iron(II) Sulfate', 'SOLID', '{"color_hex": "#90EE90", "crystal": "green"}', TRUE),
('Fe2(SO4)3', 'Iron(III) Sulfate', 'SOLID', '{"color_hex": "#DEB887", "powder": true}', TRUE),
('NO2', 'Nitrogen Dioxide', 'GAS', '{"color_hex": "#8B4500", "toxic": true, "description": "Brown Gas"}', TRUE),
('SO3', 'Sulfur Trioxide', 'GAS', '{"color_hex": "#FFFFFF", "fuming": true}', TRUE),
('H2S', 'Hydrogen Sulfide', 'GAS', '{"smell": "rotten_egg", "toxic": true}', TRUE),
-- LƯU Ý: Đã xóa dòng insert 'Sugar (Sucrose)' ở đây vì đã có ở mục 6. Hữu cơ
('MnO2', 'Manganese(II) Dioxide', 'SOLID', '{"color_hex": "#000000", "catalyst": true, "powder": true}', TRUE),
('K2Cr2O7', 'Potassium Dichromate', 'SOLID', '{"color_hex": "#FF4500", "toxic": true, "crystal": "orange"}', TRUE),
('Cr2O3', 'Chromium(III) Oxide', 'SOLID', '{"color_hex": "#006400", "powder": true, "ash": true}', TRUE);


-- ==========================================
-- D. REACTIONS (TOÀN BỘ 20 PHẢN ỨNG)
-- ==========================================

-- 1. Đốt cháy Metan
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Đốt cháy khí Metan', 'Tia lửa điện', '{"effect": "FIRE", "flame_color": "BLUE"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'CH4'), 'REACTANT', 1, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'O2'), 'REACTANT', 2, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'CO2'), 'PRODUCT', 1, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'PRODUCT', 2, 'GAS');

-- 2. Nhiệt phân thuốc tím
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Nhiệt phân thuốc tím điều chế Oxy', 'Nhiệt độ cao', '{"effect": "SMOKE", "gas_name": "O2"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'KMnO4'), 'REACTANT', 2, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'O2'), 'PRODUCT', 1, 'GAS');

-- 3. Đốt cháy Natri
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Đốt cháy Natri', 'Không khí', '{"effect": "FIRE", "flame_color": "YELLOW_ORANGE"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Na'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'O2'), 'REACTANT', 1, 'GAS');

-- 4. Trung hòa Axit-Bazo
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Trung hòa NaOH và HCl', 'Thường', '{"effect": "HEAT"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaOH'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'HCl'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaCl'), 'PRODUCT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'PRODUCT', 1, 'LIQUID');

-- 5. Sắt tác dụng Axit
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Sắt tan trong Axit', 'Thường', '{"effect": "BUBBLES", "solution_color": "PALE_GREEN"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Fe'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'HCl'), 'REACTANT', 2, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'FeCl3'), 'PRODUCT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2'), 'PRODUCT', 1, 'GAS');

-- 6. Mưa Vàng
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Phản ứng Mưa Vàng (Golden Rain)', 'Trộn dung dịch', '{"effect": "PRECIPITATE", "ppt_color": "#FFD700", "particle_effect": "SHIMMER"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Pb(NO3)2'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'KI'), 'REACTANT', 2, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'PbI2'), 'PRODUCT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'KNO3'), 'PRODUCT', 2, 'AQ');

-- 7. Tạo khói trắng
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Tạo khói trắng từ hai khí không màu', 'Đưa hai miệng lọ lại gần nhau', '{"effect": "SMOKE", "smoke_color": "#FFFFFF", "density": "THICK"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NH3'), 'REACTANT', 1, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'HCl'), 'REACTANT', 1, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NH4Cl'), 'PRODUCT', 1, 'SOLID');

-- 8. Phenolphthalein
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Nhận biết Bazơ bằng Phenolphthalein', 'Nhỏ vài giọt chỉ thị', '{"effect": "COLOR_CHANGE", "from_color": "CLEAR", "to_color": "#FF00FF"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaOH'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Phenol'), 'CATALYST', 0, 'LIQUID');

-- 9. Sắt đẩy Đồng
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Sắt tác dụng với dung dịch Đồng(II) Sunfat', 'Ngâm đinh sắt', '{"effect": "METAL_COATING", "surface_color": "#B87333", "solution_fade": "BLUE_TO_LIGHT_GREEN"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Fe'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'CuSO4'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'FeCl3'), 'PRODUCT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cu'), 'PRODUCT', 1, 'SOLID');

-- 10. Kết tủa Nâu đỏ
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Tạo kết tủa Sắt(III) Hydroxit', 'Trộn dung dịch', '{"effect": "PRECIPITATE", "ppt_color": "#8B4513", "ppt_name": "Fe(OH)3"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'FeCl3'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaOH'), 'REACTANT', 3, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Fe(OH)3'), 'PRODUCT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaCl'), 'PRODUCT', 3, 'AQ');

-- 11. Kem đánh răng voi
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Thí nghiệm Kem đánh răng voi', 'Dùng xúc tác', '{"effect": "FOAM_EXPLOSION", "foam_color": "#FFFFFF", "expansion_speed": "VERY_FAST"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O2'), 'REACTANT', 2, 'LIQUID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'MnO2'), 'CATALYST', 0, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'PRODUCT', 2, 'LIQUID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'O2'), 'PRODUCT', 1, 'GAS');

-- 12. Rắn đen trỗi dậy
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Phản ứng Rắn đen (Carbon Snake)', 'Axit Sulfuric đặc nhỏ vào đường', '{"effect": "GROWING_COLUMN", "material_color": "#000000"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'C12H22O11'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2SO4'), 'REACTANT', 1, 'LIQUID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'C'), 'PRODUCT', 12, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'PRODUCT', 11, 'GAS');

-- 13. Núi lửa phun trào (Cần insert chất NH4-Cr2O7 trước)
INSERT INTO chemicals (formula, common_name, state_at_room_temp, molecular_data, is_public) 
VALUES ('(NH4)2Cr2O7', 'Ammonium Dichromate', 'SOLID', '{"color_hex": "#FF8C00", "crystal": "orange"}', TRUE);

WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Mô phỏng núi lửa phun trào', 'Mồi lửa', '{"effect": "VOLCANO", "spark_color": "#FF4500", "ash_color": "#006400"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = '(NH4)2Cr2O7'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'N2'), 'PRODUCT', 1, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'PRODUCT', 4, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cr2O3'), 'PRODUCT', 1, 'SOLID');

-- 14. Đồng tác dụng Axit Nitric
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Đồng tan trong Axit Nitric đặc', 'Trong tủ hút', '{"effect": "GAS_EVOLUTION", "gas_color": "#8B4500", "solution_change": "CLEAR_TO_GREEN_BLUE"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cu'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'HNO3'), 'REACTANT', 4, 'LIQUID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cu(NO3)2'), 'PRODUCT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NO2'), 'PRODUCT', 2, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'PRODUCT', 2, 'LIQUID');

-- 15. Cây Bạc
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Phản ứng Cây Bạc', 'Thả dây đồng vào dung dịch Bạc', '{"effect": "CRYSTAL_GROWTH", "crystal_color": "#C0C0C0", "solution_color": "#1E90FF"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cu'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'AgNO3'), 'REACTANT', 2, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cu(NO3)2'), 'PRODUCT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Ag'), 'PRODUCT', 2, 'SOLID');

-- 16. Nhiệt nhôm
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Phản ứng Nhiệt nhôm', 'Mồi lửa Magie', '{"effect": "BLINDING_LIGHT", "sparks": true, "molten_metal": true}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Al'), 'REACTANT', 2, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Fe2O3'), 'REACTANT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Al2O3'), 'PRODUCT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Fe'), 'PRODUCT', 2, 'LIQUID');

-- 17. Nhận biết H2S (Cần insert PbS trước)
INSERT INTO chemicals (formula, common_name, state_at_room_temp, molecular_data, is_public) 
VALUES ('PbS', 'Lead(II) Sulfide', 'SOLID', '{"color_hex": "#000000", "precipitate": true}', TRUE);

WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Nhận biết H2S bằng chì', 'Giấy lọc tẩm chì', '{"effect": "SURFACE_DARKENING", "target_color": "#000000"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Pb(NO3)2'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2S'), 'REACTANT', 1, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'PbS'), 'PRODUCT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'HNO3'), 'PRODUCT', 2, 'AQ');

-- 18. Nhôm tan trong kiềm
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Hòa tan Nhôm bằng Kiềm', 'Nhiệt độ phòng', '{"effect": "DISSOLVING_METAL", "bubbles": true, "gas_name": "H2"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Al'), 'REACTANT', 2, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaOH'), 'REACTANT', 2, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2O'), 'REACTANT', 2, 'LIQUID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'NaAlO2'), 'PRODUCT', 2, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2'), 'PRODUCT', 3, 'GAS');

-- 19. Kết tủa BaSO4
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Nhận biết Axit Sulfuric', 'Thường', '{"effect": "PRECIPITATE", "ppt_color": "#FFFFFF"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'H2SO4'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'BaCl2'), 'REACTANT', 1, 'AQ'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'BaSO4'), 'PRODUCT', 1, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'HCl'), 'PRODUCT', 2, 'AQ');

-- 20. Sắt cháy trong Clo
WITH r AS ( INSERT INTO reactions (description, conditions, visual_config) VALUES ('Đốt cháy dây sắt trong Clo', 'Mồi lửa', '{"effect": "SMOKE_AND_SPARKS", "smoke_color": "#A0522D"}') RETURNING id )
INSERT INTO reaction_components (reaction_id, chemical_id, role, coefficient, state_in_reaction) VALUES
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Fe'), 'REACTANT', 2, 'SOLID'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'Cl2'), 'REACTANT', 3, 'GAS'),
((SELECT id FROM r), (SELECT id FROM chemicals WHERE formula = 'FeCl3'), 'PRODUCT', 2, 'SOLID');