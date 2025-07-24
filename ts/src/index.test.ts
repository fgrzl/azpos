


import { describe, it, expect } from 'vitest';
import { midpoint, validate, needsRebalance, rebalance3, compare } from '../src/index';
import expectations from '../../.test-vectors/expectations.json';

type MidpointCase = { a: string; b: string; expected?: string; error?: string };
type ValidateCase = { pos: string; desc?: string; error?: string };
type NeedsRebalanceCase = { a: string; b: string };
type Rebalance3Case = { a: string; c: string; expected: string[] };
type CompareCase = { a: string; b: string; result: number };
type ReservedCase = { assigned: string; error: string };

describe('azpos expectations.json test suite', () => {
  describe('midpoint', () => {
    expectations.midpoint.positive.forEach(({ a, b, expected }: MidpointCase) => {
      it(`midpoint(${JSON.stringify(a)}, ${JSON.stringify(b)}) === ${JSON.stringify(expected)}`, () => {
        expect(midpoint(a, b)).toBe(expected);
      });
    });
    expectations.midpoint.negative.forEach(({ a, b, error }: MidpointCase) => {
      it(`midpoint(${JSON.stringify(a)}, ${JSON.stringify(b)}) throws ${error}`, () => {
        expect(() => midpoint(a, b)).toThrow();
      });
    });
  });

  describe('validate', () => {
    expectations.validate.positive.forEach(({ pos }: ValidateCase) => {
      it(`validate(${JSON.stringify(pos)}) === true`, () => {
        expect(validate(pos)).toBe(true);
      });
    });
    expectations.validate.negative.forEach(({ pos }: ValidateCase) => {
      it(`validate(${JSON.stringify(pos)}) === false`, () => {
        expect(validate(pos)).toBe(false);
      });
    });
  });

  describe('needsRebalance', () => {
    expectations.needsRebalance.positive.forEach(({ a, b }: NeedsRebalanceCase) => {
      it(`needsRebalance(${JSON.stringify(a)}, ${JSON.stringify(b)}) === true`, () => {
        expect(needsRebalance(a, b)).toBe(true);
      });
    });
    expectations.needsRebalance.negative.forEach(({ a, b }: NeedsRebalanceCase) => {
      it(`needsRebalance(${JSON.stringify(a)}, ${JSON.stringify(b)}) === false`, () => {
        expect(needsRebalance(a, b)).toBe(false);
      });
    });
  });

  describe('rebalance3', () => {
    expectations.rebalance3.forEach(({ a, c, expected }: Rebalance3Case) => {
      it(`rebalance3(${JSON.stringify(a)}, ${JSON.stringify(c)}) returns ${JSON.stringify(expected)}`, () => {
        expect(rebalance3(a, c)).toEqual(expected);
      });
    });
  });

  describe('compare', () => {
    expectations.compare.forEach(({ a, b, result }: CompareCase) => {
      it(`compare(${JSON.stringify(a)}, ${JSON.stringify(b)}) === ${result}`, () => {
        expect(compare(a, b)).toBe(result);
      });
    });
  });

  describe('reserved bounds', () => {
    expectations.reserved.forEach(({ assigned, error }: ReservedCase) => {
      it(`reserved position: ${assigned} should be invalid`, () => {
        expect(validate(assigned)).toBe(false);
      });
    });
  });
});

