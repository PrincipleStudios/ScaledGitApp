import { describe, it, expect, expectTypeOf } from 'vitest';
import type { ElementTemplate } from './types';

export function describeElementTemplate<T>(target: ElementTemplate<T>) {
	describe(target.displayName, () => {
		it(`matches the original element's signature`, () => {
			expect(target).not.toBeNull();
			expectTypeOf(target).toBeFunction();
			expectTypeOf(target).returns.toMatchTypeOf<React.ReactNode>();
		});
	});
}
