# Old Biscayne Designs Website Requirements Document

## 1. Project Overview

**Project Name:** Old Biscayne Designs Product Catalog
**Platform:** Umbraco 15.4.3 with Shadcn UI
**Purpose:** Product catalog website to replace existing oldbiscaynedesigns.com (no e-commerce functionality)
**Reference Site:** [oldbiscaynedesigns.com](https://oldbiscaynedesigns.com)

## 2. Business Goals

- Recreate and enhance the existing oldbiscaynedesigns.com product catalog
- Maintain brand identity and visual style of oldbiscaynedesigns.com
- Showcase products in an organized, visually appealing catalog
- Provide detailed product information to potential customers
- Generate leads through contact forms
- Modernize technology stack while preserving existing content and functionality

## 3. Content Requirements

### 3.1 Content Types

1. **Products**
   - Hierarchical organization by product categories (Beds, Wall Units, Office, etc.)
   - Product attributes:
     - Name (e.g., "Aaden Queen")
     - Product ID/SKU (e.g., "26587D")
     - Product image(s)
     - Dimensions (Width, Height, Length)
     - Component measurements (e.g., Headboard Height, Footboard Height)
     - Available finishes
     - Custom size availability
     - PDF catalog page link
   - Support for product variations (e.g., King, Queen, Twin sizes)

2. **Product Categories**
   - Main categories from existing site:
     - New Releases
     - Beds (largest category with many subcategories)
     - Wall Units
     - Storage Cabinets
     - Credenzas, Chests, End Tables
     - Office
     - Occasionals
     - Dining Tables
     - Seating
     - Hardware
     - Finishes & Fabrics
     - Lifestyle Gallery
   - Support for featured products within categories

3. **Pages**
   - Home page with featured products/lifestyle images
   - About Us page with company history and team information
   - Find a Dealer page
   - Contact page
   - Login page for dealers/staff

4. **Forms**
   - Newsletter subscription (First Name, Email)
   - Contact form
   - Dealer inquiry form

## 4. Design Requirements

- Implement Shadcn UI while maintaining the visual aesthetic of oldbiscaynedesigns.com
- Preserve brand colors, typography, and overall design language
- Ensure consistent user experience between old and new sites
- Clean, elegant design that showcases the craftsmanship of the furniture
- Mobile responsive design for all pages
- Maintain the company tagline: "Craftsmanship is a benediction over good design. - Gabrielli"

## 5. Technical Requirements

- Built on Umbraco 15.4.3 CMS
- Shadcn UI for component styling
- Responsive design for mobile, tablet, and desktop
- Search functionality for products
- PDF generation/linking for product catalog pages
- Newsletter subscription integration
- Contact form with email notifications
- SEO optimization for product pages

## 6. User Journeys

1. **Product Browsing**
   - Navigate categories > View product listings > View product details
   - Filter products by attributes (size, style, etc.)
   - Search for specific products

2. **Dealer Location**
   - Find dealer page > Search by location > Contact dealer

3. **Information Request**
   - View product > Contact for more information
   - Subscribe to newsletter for updates

## 7. Integration Requirements

- Email service for form submissions and newsletter subscriptions
- PDF generation/storage for catalog pages
- Social media links (Facebook, Houzz, Instagram)

## 8. Content Migration

- Transfer existing product catalog (200+ products)
- Migrate product images and specifications
- Preserve existing PDF catalog pages
- Migrate company information and team details

## 9. Development Phases

### Phase 1: Setup and Structure
- Set up Umbraco 15.4.3 environment
- Configure Shadcn UI
- Create document types for products and categories
- Implement basic templates and layouts

### Phase 2: Content Migration
- Migrate product categories
- Import product data and images
- Set up PDF storage

### Phase 3: UI Development
- Implement responsive design
- Create product listing and detail pages
- Develop search functionality
- Build forms and contact pages

### Phase 4: Testing and Refinement
- Cross-browser testing
- Mobile responsiveness testing
- Performance optimization
- Content review and adjustments

### Phase 5: Launch and Training
- Final QA
- Content editor training
- Site launch
- Post-launch support

## 10. Maintenance Plan

- Regular Umbraco updates
- Content additions and updates
- Performance monitoring
- Backup strategy

## 11. Success Criteria

- All existing products successfully migrated
- Site loads within industry standard timeframes
- Forms function correctly and send notifications
- Search returns relevant results
- Mobile experience matches desktop quality
- Administrators can easily add/edit products 