import { render } from '@testing-library/react';
import { describe, it, expect, expectTypeOf } from 'vitest';
import { elementTemplate } from './elementTemplate';
import { describeElementTemplate } from './test-utils';
import type { PropsOf } from './types';

describe('elementTemplate', () => {
	describe('with a basic HTML element', () => {
		const Basic = elementTemplate('Basic', 'div', (T) => <T />);

		describeElementTemplate(Basic);

		it(`matches the original element's signature`, () => {
			expect(Basic).not.toBeNull();
			expectTypeOf(Basic).toBeFunction();
			expectTypeOf(Basic).returns.toMatchTypeOf<React.ReactNode>();
			expectTypeOf(Basic)
				.parameter(0)
				.toMatchTypeOf<JSX.IntrinsicElements['div']>();
		});

		it(`renders the raw HTML`, () => {
			const { asFragment } = render(<Basic />);
			const actual = asFragment().firstChild as HTMLElement;
			expect(actual.tagName).toBe('DIV');
		});

		it(`renders the raw HTML and passes through attributes`, () => {
			const { asFragment } = render(<Basic className="foo" aria-label="bar" />);
			const actual = asFragment().firstChild as HTMLElement;
			expect(actual.tagName).toBe('DIV');
			expect(actual.className).toBe('foo');
			expect(actual.getAttribute('aria-label')).toBe('bar');
		});

		it(`renders the raw HTML and passes through children`, () => {
			const { getByRole, container } = render(
				<Basic>
					<button>Test</button>
				</Basic>,
			);

			const actual = getByRole('button');
			expect(container).toContainElement(actual);
		});

		it('has a display name', () => {
			expect(Basic.displayName).toEqual('Basic');
		});

		describe('when extended', () => {
			const Styled = Basic.extend('Styled', (T) => (
				<T className="w-full" style={{ color: 'red' }} />
			));

			it('renders the default className', () => {
				const { asFragment } = render(<Styled />);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual).toHaveClass('w-full');
			});

			it('merges classNames', () => {
				const { asFragment } = render(<Styled className="h-full" />);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual).toHaveClass('w-full');
				expect(actual).toHaveClass('h-full');
			});

			it('uses tailwind-merge to override classNames', () => {
				const { asFragment } = render(<Styled className="w-64 h-full" />);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual.classList).not.toContain('w-full');
				expect(actual).toHaveClass('w-64');
				expect(actual).toHaveClass('h-full');
			});

			it('renders the default style', () => {
				const { asFragment } = render(<Styled />);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual.style.color).toBe('red');
			});

			it('merges styles', () => {
				const { asFragment } = render(
					<Styled style={{ backgroundColor: 'yellow' }} />,
				);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual.style.color).toBe('red');
				expect(actual.style.backgroundColor).toBe('yellow');
			});

			it('allows overriding styles', () => {
				const { asFragment } = render(
					<Styled style={{ color: 'black', backgroundColor: 'yellow' }} />,
				);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual.style.color).toBe('black');
				expect(actual.style.backgroundColor).toBe('yellow');
			});

			it('uses the new display name and preserves the original', () => {
				expect(Basic.displayName).toEqual('Basic');
				expect(Styled.displayName).toEqual('Styled');
			});
		});
	});

	describe('with a React Component', () => {
		function Inner({
			id,
			children,
		}: {
			id?: string;
			children?: React.ReactNode;
		}) {
			return <section id={id}>{children}</section>;
		}
		const Basic = elementTemplate('Basic', Inner, (T) => <T />);

		it(`matches the original element's signature`, () => {
			expect(Basic).not.toBeNull();
			expectTypeOf(Basic).toBeFunction();
			expectTypeOf(Basic).parameter(0).toMatchTypeOf<PropsOf<typeof Inner>>();
		});

		it(`renders the raw HTML`, () => {
			const { asFragment } = render(<Basic />);
			const actual = asFragment().firstChild as HTMLDivElement;
			expect(actual.tagName).toBe('SECTION');
		});

		it(`renders the raw HTML and passes through attributes`, () => {
			const { asFragment } = render(<Basic id="foo" />);
			const actual = asFragment().firstChild as HTMLDivElement;
			expect(actual.tagName).toBe('SECTION');
			expect(actual.id).toBe('foo');
		});

		it(`renders the raw HTML and passes through children`, () => {
			const { getByRole, container } = render(
				<Basic>
					<button>Test</button>
				</Basic>,
			);

			const actual = getByRole('button');
			expect(container).toContainElement(actual);
		});

		describe('when extended', () => {
			const IdSection = Basic.extend('IdSection', (T) => <T id="foo" />);

			it('renders the default properties', () => {
				const { asFragment } = render(<IdSection />);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual.id).toBe('foo');
			});

			it('allows overriding properties as the original', () => {
				const { asFragment } = render(<IdSection id="bar" />);
				const actual = asFragment().firstChild as HTMLElement;
				expect(actual.id).toBe('bar');
			});

			it('uses the new display name', () => {
				expect(IdSection.displayName).toEqual('IdSection');
			});
		});
	});

	describe('supporting themes', () => {
		const Button = elementTemplate('Button', 'button', (T) => (
			<T
				type="button"
				className="bg-blue-700 hover:bg-blue-950 focus:bg-blue-950 hover:ring focus:ring text-white"
			/>
		)).themed({
			Secondary: (T) => (
				<T className="bg-blue-200 hover:bg-blue-50 focus:bg-blue-50 text-blue-950" />
			),
		});

		it('renders the appropriate theme', () => {
			const { asFragment } = render(<Button.Secondary />);
			const actual = asFragment().firstChild as HTMLElement;
			expect(actual).toHaveClass('bg-blue-200');
			expect(actual).toHaveClass('hover:bg-blue-50');
			expect(actual).toHaveClass('focus:bg-blue-50');
			expect(actual).toHaveClass('text-blue-950');
		});

		it('preserves untouched classes', () => {
			const { asFragment } = render(<Button.Secondary />);
			const actual = asFragment().firstChild as HTMLElement;
			expect(actual).toHaveClass('hover:ring');
			expect(actual).toHaveClass('focus:ring');
		});

		it('creates an appropriate display name', () => {
			expect(Button.Secondary.displayName);
			const { asFragment } = render(<Button.Secondary />);
			const actual = asFragment().firstChild as HTMLElement;
			expect(actual).toHaveClass('bg-blue-200');
			expect(actual).toHaveClass('focus:ring');
		});
	});
});
