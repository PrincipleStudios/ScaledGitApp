import { elementTemplate } from '../templating';

export const BulletList = Object.assign(
	elementTemplate('BulletList', 'ul', (T) => <T className="list-disc ml-6" />),
	{
		Item: elementTemplate('BulletList.Item', 'li', (T) => <T />),
	},
);
