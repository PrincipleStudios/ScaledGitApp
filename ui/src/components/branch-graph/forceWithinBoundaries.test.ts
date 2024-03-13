import { describe, it, expect } from 'vitest';
import { toOffsetScale } from './forceWithinBoundaries';

describe('toOffsetScale', () => {
	it('keeps target above min', () => {
		const [actualOffset, actualScale] = toOffsetScale(-100, 100, 300, 0);
		expect(actualOffset).toBe(100);
		expect(actualScale).toBe(1);
	});

	it('adjusts scale', () => {
		const [actualOffset, actualScale] = toOffsetScale(0, 400, 300, 0);
		expect(actualOffset).toBe(0);
		expect(actualScale).toBe(0.75);
	});

	it('keeps target below max', () => {
		const [actualOffset, actualScale] = toOffsetScale(200, 400, 300, 0);
		expect(actualOffset).toBe(-100);
		expect(actualScale).toBe(1);
	});

	it('adjusts position and scale', () => {
		const [actualOffset, actualScale] = toOffsetScale(-100, 400, 300, 0);
		expect(actualOffset).toBe(60);
		expect(actualScale).toBe(0.6);
	});
});
