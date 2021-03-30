using Bot.Skills.Dialogs.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills.Dialogs
{
    public class TestSkill2Manifest : SkillManifestBase
    {
        public TestSkill2Manifest(IOptions<SkillManifestOptions> options) : base(options)
        {
            SetRelatedDialogType(typeof(TestSkill2Dialog));
        }

        public override string name => "Test.Skill2";

        public override string description => "Responds with a message indicating Test Skill 2 was successfully called.";

        public override string version => "1.0";
    }

    public class TestSkill2Dialog : DoActionWithMessageResponseBase<TestSkill1Dialog, bool>
    {
        public TestSkill2Dialog() : base()
        {
        }

        protected override bool DoAction(out string actionResultText)
        {
            actionResultText = "Response From Test Skill 2.";
            return true;
        }
    }
}
