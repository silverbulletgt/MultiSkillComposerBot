using Bot.Skills.Dialogs.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills.Dialogs
{
    public class TestSkill1Manifest : SkillManifestBase
    {
        public TestSkill1Manifest(IOptions<SkillManifestOptions> options) : base(options)
        {
            SetRelatedDialogType(typeof(TestSkill1Dialog));
        }

        public override string name => "Test.Skill1";

        public override string description => "Responds with a message indicating Test Skill 1 was successfully called.";

        public override string version => "1.0";
    }

    public class TestSkill1Dialog : DoActionWithMessageResponseBase<TestSkill1Dialog, bool>
    {
        public TestSkill1Dialog() : base()
        {
        }

        protected override bool DoAction(out string actionResultText)
        {
            actionResultText = "Response From Test Skill 1.";
            return true;
        }
    }
}
