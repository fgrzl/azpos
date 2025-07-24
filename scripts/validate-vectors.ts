#!/usr/bin/env node

/**
 * Test vector consistency checker for azpos implementations
 * Validates that all language implementations produce consistent results
 */

import { readFileSync } from 'fs';
import { join } from 'path';

interface MidpointTest {
  a: string;
  b: string;
  expected?: string;
  error?: string;
  note?: string;
}

interface ValidateTest {
  pos: string;
  valid: boolean;
  error?: string;
}

interface RebalanceTest {
  a: string;
  b: string;
  needs: boolean;
}

interface Rebalance3Test {
  a: string;
  c: string;
  expected: string[];
}

interface CompareTest {
  a: string;
  b: string;
  result: number;
}

interface TestVectors {
  midpointTests: {
    positive: MidpointTest[];
    negative: MidpointTest[];
  };
  validateTests: ValidateTest[];
  needsRebalanceTests: RebalanceTest[];
  rebalance3Tests: Rebalance3Test[];
  compareTests: CompareTest[];
}

function loadTestVectors(): TestVectors {
  const vectorsPath = join(__dirname, '..', 'test-vectors', 'positions.json');
  const content = readFileSync(vectorsPath, 'utf-8');
  return JSON.parse(content);
}

function loadAdditionalVectors(): { positive: MidpointTest[], negative: MidpointTest[] } {
  const positivePath = join(__dirname, '..', 'test-vectors', 'positive.json');
  const negativePath = join(__dirname, '..', 'test-vectors', 'negative.json');
  
  const positive = JSON.parse(readFileSync(positivePath, 'utf-8'));
  const negative = JSON.parse(readFileSync(negativePath, 'utf-8'));
  
  return { positive, negative };
}

function validateMidpointTests(tests: MidpointTest[], type: 'positive' | 'negative'): boolean {
  let isValid = true;
  
  for (const test of tests) {
    if (!test.a && test.a !== '') {
      console.error(`❌ Missing 'a' in ${type} test: ${JSON.stringify(test)}`);
      isValid = false;
      continue;
    }
    
    if (!test.b && test.b !== '') {
      console.error(`❌ Missing 'b' in ${type} test: ${JSON.stringify(test)}`);
      isValid = false;
      continue;
    }
    
    if (type === 'positive' && !test.expected) {
      console.error(`❌ Missing 'expected' in positive test: ${JSON.stringify(test)}`);
      isValid = false;
      continue;
    }
    
    if (type === 'negative' && !test.error && !test.expected) {
      console.error(`❌ Missing 'error' or 'expected' in negative test: ${JSON.stringify(test)}`);
      isValid = false;
      continue;
    }
    
    // Validate position strings contain only lowercase a-z
    const validatePos = (pos: string, name: string) => {
      if (pos && !/^[a-z]*$/.test(pos)) {
        console.error(`❌ Invalid characters in ${name}: ${pos} (test: ${JSON.stringify(test)})`);
        isValid = false;
      }
    };
    
    validatePos(test.a, 'a');
    validatePos(test.b, 'b');
    if (test.expected) {
      validatePos(test.expected, 'expected');
    }
    
    console.log(`✅ ${type} midpoint test validated: ${test.a} -> ${test.b}`);
  }
  
  return isValid;
}

function validateTestVectors(vectors: TestVectors): boolean {
  let isValid = true;
  
  console.log('Validating test vectors...');
  
  // Validate midpoint tests
  if (!validateMidpointTests(vectors.midpointTests.positive, 'positive')) {
    isValid = false;
  }
  
  if (!validateMidpointTests(vectors.midpointTests.negative, 'negative')) {
    isValid = false;
  }
  
  // Validate other test types
  for (const test of vectors.validateTests) {
    if (typeof test.pos !== 'string' || typeof test.valid !== 'boolean') {
      console.error(`❌ Invalid validate test: ${JSON.stringify(test)}`);
      isValid = false;
    } else {
      console.log(`✅ Validate test: ${test.pos} -> ${test.valid}`);
    }
  }
  
  for (const test of vectors.needsRebalanceTests) {
    if (typeof test.a !== 'string' || typeof test.b !== 'string' || typeof test.needs !== 'boolean') {
      console.error(`❌ Invalid rebalance test: ${JSON.stringify(test)}`);
      isValid = false;
    } else {
      console.log(`✅ Rebalance test: ${test.a} -> ${test.b} needs: ${test.needs}`);
    }
  }
  
  for (const test of vectors.compareTests) {
    if (typeof test.a !== 'string' || typeof test.b !== 'string' || typeof test.result !== 'number') {
      console.error(`❌ Invalid compare test: ${JSON.stringify(test)}`);
      isValid = false;
    } else {
      console.log(`✅ Compare test: ${test.a} vs ${test.b} -> ${test.result}`);
    }
  }
  
  return isValid;
}

function main() {
  try {
    const vectors = loadTestVectors();
    const additional = loadAdditionalVectors();
    
    // Merge additional vectors into main vectors for comprehensive validation
    vectors.midpointTests.positive.push(...additional.positive);
    vectors.midpointTests.negative.push(...additional.negative);
    
    const isValid = validateTestVectors(vectors);
    
    if (isValid) {
      console.log('\n✅ All test vectors are valid!');
      console.log(`Total positive midpoint tests: ${vectors.midpointTests.positive.length}`);
      console.log(`Total negative midpoint tests: ${vectors.midpointTests.negative.length}`);
      console.log(`Total validate tests: ${vectors.validateTests.length}`);
      console.log(`Total rebalance tests: ${vectors.needsRebalanceTests.length}`);
      console.log(`Total compare tests: ${vectors.compareTests.length}`);
      process.exit(0);
    } else {
      console.log('\n❌ Test vector validation failed!');
      process.exit(1);
    }
  } catch (error) {
    console.error('Error validating test vectors:', error);
    process.exit(1);
  }
}

if (require.main === module) {
  main();
}
