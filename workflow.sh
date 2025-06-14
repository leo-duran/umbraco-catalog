#!/bin/bash

# Set colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${YELLOW}=== Starting Umbraco Development Workflow ===${NC}"

# Step 1: Build the project
echo -e "${YELLOW}=== Step 1: Building the project ===${NC}"
dotnet build
BUILD_RESULT=$?

if [ $BUILD_RESULT -ne 0 ]; then
    echo -e "${RED}Build failed! Please fix the errors and try again.${NC}"
    exit 1
fi

echo -e "${GREEN}Build completed successfully!${NC}"

# Step 2: Run the cycle script
echo -e "${YELLOW}=== Step 2: Running the cycle script ===${NC}"
./cycle.sh
CYCLE_RESULT=$?

if [ $CYCLE_RESULT -ne 0 ]; then
    echo -e "${RED}Cycle script failed! Please check the logs.${NC}"
    exit 1
fi

echo -e "${GREEN}Cycle script completed successfully!${NC}"

# Add a delay to ensure all document types and templates are created
echo -e "${YELLOW}=== Waiting for 10 seconds to ensure all document types and templates are created ===${NC}"
sleep 10

# Step 3: Verify the database
echo -e "${YELLOW}=== Step 3: Verifying the database ===${NC}"

# Create output directory if it doesn't exist
mkdir -p ./docs/verification

# Get current timestamp for the output file
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
OUTPUT_FILE="./docs/verification/db_verification_${TIMESTAMP}.md"

echo "# Umbraco Database Verification - $(date)" > $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Document Types" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT un.id, un.text, ct.alias, ct.icon FROM umbracoNode un JOIN cmsContentType ct ON un.id = ct.nodeId WHERE un.nodeObjectType = 'A2CB7800-F571-4787-9638-BC48539A0EFB' ORDER BY ct.alias;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Document Type Inheritance" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT parent.alias as parentAlias, child.alias as childAlias FROM cmsContentType2ContentType c2c JOIN cmsContentType parent ON c2c.parentContentTypeId = parent.nodeId JOIN cmsContentType child ON c2c.childContentTypeId = child.nodeId ORDER BY parent.alias;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Templates" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT un.id, t.alias, un.text FROM umbracoNode un JOIN cmsTemplate t ON un.id = t.nodeId ORDER BY t.alias;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Document Type to Template Mapping" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT ct.alias as docTypeAlias, t.alias as templateAlias FROM cmsContentType ct JOIN cmsTemplate t ON ct.defaultTemplateId = t.nodeId WHERE ct.defaultTemplateId IS NOT NULL ORDER BY ct.alias;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Property Groups" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT ptg.id, ptg.text, ptg.alias, ct.alias as contentTypeAlias FROM cmsPropertyTypeGroup ptg JOIN cmsContentType ct ON ptg.contenttypeNodeId = ct.nodeId ORDER BY ct.alias, ptg.sortorder;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Properties" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT pt.id, pt.Name, pt.Alias, ct.alias as contentTypeAlias FROM cmsPropertyType pt JOIN cmsContentType ct ON pt.contentTypeId = ct.nodeId ORDER BY ct.alias, pt.sortOrder;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Template Files" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
find src/Catalog/Views -name "*.cshtml" | sort >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE

echo -e "${GREEN}Database verification completed. Results saved to $OUTPUT_FILE${NC}"

# Check for expected document types
echo -e "${YELLOW}=== Checking for expected document types ===${NC}"
EXPECTED_TYPES=("catalogPage" "product" "aboutUsPage" "productCategory" "productDetailPage")
MISSING_TYPES=()

for DOCTYPE in "${EXPECTED_TYPES[@]}"; do
    COUNT=$(sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT COUNT(*) FROM cmsContentType WHERE alias = '$DOCTYPE';")
    if [ "$COUNT" -eq "0" ]; then
        MISSING_TYPES+=("$DOCTYPE")
        echo -e "${RED}Missing document type: $DOCTYPE${NC}"
    else
        echo -e "${GREEN}Found document type: $DOCTYPE${NC}"
    fi
done

# Check for expected templates
echo -e "${YELLOW}=== Checking for expected templates ===${NC}"
EXPECTED_TEMPLATES=("catalogPageTemplate" "aboutUsTemplate" "productCategoryTemplate" "productDetailTemplate")
MISSING_TEMPLATES=()

for TEMPLATE in "${EXPECTED_TEMPLATES[@]}"; do
    COUNT=$(sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT COUNT(*) FROM cmsTemplate WHERE alias = '$TEMPLATE';")
    if [ "$COUNT" -eq "0" ]; then
        MISSING_TEMPLATES+=("$TEMPLATE")
        echo -e "${RED}Missing template: $TEMPLATE${NC}"
    else
        echo -e "${GREEN}Found template: $TEMPLATE${NC}"
    fi
done

# Summary
echo -e "${YELLOW}=== Workflow Summary ===${NC}"
if [ ${#MISSING_TYPES[@]} -eq 0 ] && [ ${#MISSING_TEMPLATES[@]} -eq 0 ]; then
    echo -e "${GREEN}All document types and templates were created successfully!${NC}"
else
    echo -e "${RED}Some document types or templates are missing:${NC}"
    if [ ${#MISSING_TYPES[@]} -gt 0 ]; then
        echo -e "${RED}Missing document types: ${MISSING_TYPES[*]}${NC}"
    fi
    if [ ${#MISSING_TEMPLATES[@]} -gt 0 ]; then
        echo -e "${RED}Missing templates: ${MISSING_TEMPLATES[*]}${NC}"
    fi
fi

echo -e "${YELLOW}=== Workflow Completed ===${NC}" 